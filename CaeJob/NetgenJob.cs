using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Management;
using System.ComponentModel;
using CaeGlobals;

namespace CaeJob
{
    //[Serializable]
    //public enum JobStatus
    //{
    //    None,
    //    InQueue,
    //    Running,
    //    OK,
    //    Killed,
    //    TimedOut,
    //    Failed
    //}

    [Serializable]
    public class NetgenJob : NamedClass
    {
        // Variables                                                                                                                
        protected string _workDirectory;
        protected string _executable;
        protected string _argument;
        protected JobStatus _jobStatus;
        //
        [NonSerialized] protected Stopwatch _watch;
        [NonSerialized] protected Stopwatch _updateWatch;   // timer does not tick - use update watch
        [NonSerialized] private Process _exe;
        [NonSerialized] AutoResetEvent _outputWaitHandle;
        [NonSerialized] AutoResetEvent _errorWaitHandle;
        [NonSerialized] private StringBuilder _sbOutput;
        [NonSerialized] private string _outputFileName;
        [NonSerialized] private string _errorFileName;
        [NonSerialized] private object _myLock;


        // Properties                                                                                                               
        public override string Name
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                _argument = Name;
            }
        }
        public string WorkDirectory
        {
            get { return _workDirectory; }
            set { _workDirectory = value; }
        }
        public string Executable
        {
            get { return _executable; }
            set { _executable = value; }
        }
        public string Argument
        {
            get { return _argument; }
            set { _argument = value; }
        }
        public JobStatus JobStatus { get { return _jobStatus; } }
        public string OutputData
        {
            get
            {
                try
                {
                    if (_sbOutput != null)
                    {
                        if (_myLock == null) _myLock = new object();
                        lock (_myLock) return _sbOutput.ToString();
                    }
                    else return null;
                }
                catch
                {
                    return null;
                }
            }
        }


        // Events                                                                                                                           
        public event Action<string> AppendOutput;


        // Constructor                                                                                                              
        public NetgenJob(string name, string executable, string argument, string workDirectory)
            : base(name)
        {
            _executable = executable;
            _argument = argument;
            _workDirectory = workDirectory;
            //
            _exe = null;
            _jobStatus = JobStatus.None;
            _watch = null;
            _updateWatch = null;
            _sbOutput = null;
        }

        // Event handlers                                                                                                           
       

        // Methods                                                                                                                  
        public void Submit()
        {
            if (_myLock == null) _myLock = new object();
            lock (_myLock)
            {
                if (_sbOutput == null) _sbOutput = new StringBuilder();
                _sbOutput.Clear();
            }
            //
            AddDataToOutput("Running command: " + _executable + " " + _argument);
            //
            _watch = new Stopwatch();
            _updateWatch = new Stopwatch();
            //
            _jobStatus = JobStatus.Running;
            //
            _watch.Start();
            _updateWatch.Start();
            //
            Run();
            RunCompleted();
        }
        void bwStart_DoWork(object sender, DoWorkEventArgs e)
        {
            Run();
        }
        void bwStart_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RunCompleted();
        }

        private void Run()
        {
            if (!File.Exists(_executable)) throw new Exception("The file '" + _executable + "' does not exist.");
            if (!Tools.WaitForFileToUnlock(_executable, 5000)) throw new Exception("The netgen mesher is busy.");
            //
            string tmpName = Path.GetFileName(Name);
            _outputFileName = Path.Combine(_workDirectory, "_output_" + tmpName + ".txt");
            _errorFileName = Path.Combine(_workDirectory, "_error_" + tmpName + ".txt");
            //
            if (File.Exists(_outputFileName)) File.Delete(_outputFileName);
            if (File.Exists(_errorFileName)) File.Delete(_errorFileName);
            //
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.CreateNoWindow = true;
            psi.FileName = _executable;
            psi.Arguments = _argument;
            psi.WorkingDirectory = _workDirectory;
            psi.WindowStyle = ProcessWindowStyle.Normal;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;   // sometimes needed
            psi.RedirectStandardError = true;
            //
            Debug.WriteLine(DateTime.Now + "   Start proces: " + tmpName + " in: " + _workDirectory);
            //
            _exe = new Process();
            _exe.StartInfo = psi;
            //
            //using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
            //using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
            _outputWaitHandle = new AutoResetEvent(false);
            _errorWaitHandle = new AutoResetEvent(false);
            {
                _exe.OutputDataReceived += _exe_OutputDataReceived;
                //_exe.OutputDataReceived += (sender, e) =>
                //{
                //    if (e.Data == null)
                //    {
                //        // the safe wait handle closes on kill
                //        if (!_outputWaitHandle.SafeWaitHandle.IsClosed) _outputWaitHandle.Set();
                //    }
                //    else
                //    {
                //        AddDataToOutput(e.Data);
                //    }
                //};
                //
                _exe.ErrorDataReceived += _exe_ErrorDataReceived;
                //_exe.ErrorDataReceived += (sender, e) =>
                //{
                //    if (e.Data == null)
                //    {
                //        // the safe wait handle closes on kill
                //        if (!errorWaitHandle.SafeWaitHandle.IsClosed) errorWaitHandle.Set();
                //    }
                //    else
                //    {
                //        File.AppendAllText(_errorFileName, e.Data + Environment.NewLine);
                //        AddDataToOutput(e.Data);
                //    }
                //};
                //
                _exe.Start();
                _exe.BeginOutputReadLine();
                _exe.BeginErrorReadLine();
                //
                int ms = 1000 * 3600 * 24 * 7 * 3; // 3 weeks
                //
                if (_exe.WaitForExit(ms) && _outputWaitHandle.WaitOne(ms) && _errorWaitHandle.WaitOne(ms))
                {
                    // Process completed. Check process.ExitCode here.
                    // after Kill() _jobStatus is Killed
                    if (_jobStatus == JobStatus.Running) _jobStatus = JobStatus.OK;
                }
                else
                {
                    // Timed out.
                    Kill("Time out.");
                    _jobStatus = JobStatus.TimedOut;
                }
                _exe.Close();
            }
        }

        private void _exe_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                // the safe wait handle closes on kill
                if (!_errorWaitHandle.SafeWaitHandle.IsClosed) _errorWaitHandle.Set();
            }
            else
            {
                File.AppendAllText(_errorFileName, e.Data + Environment.NewLine);
                AddDataToOutput(e.Data);
            }
        }

        private void _exe_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data == null)
            {
                // the safe wait handle closes on kill
                if (!_outputWaitHandle.SafeWaitHandle.IsClosed) _outputWaitHandle.Set();
            }
            else
            {
                AddDataToOutput(e.Data);
            }
        }

        void RunCompleted()
        {
            _watch.Stop();
            _updateWatch.Stop();
            //
            AddDataToOutput("");
            AddDataToOutput("Elapsed time [s]: " + _watch.Elapsed.TotalSeconds.ToString());
            //
            UpdateOutput();
            // Dereference the link to otheh objects
            AppendOutput = null;
        }

        public void Kill(string message)
        {
            try
            {
                if (_exe != null)
                {
                    AddDataToOutput(message);
                    //
                    _watch.Stop();
                    _updateWatch.Stop();
                    //
                    _jobStatus = JobStatus.Killed;  // this has to be here before _exe.Kill, to return the correct status
                    //
                    KillAllProcessesSpawnedBy((UInt32)_exe.Id);
                    //
                    _exe.Kill();
                }
            }
            catch
            { }
            finally
            {
                // Dereference the link to otheh objects
                AppendOutput = null;
            }
        }
        private static void KillAllProcessesSpawnedBy(UInt32 parentProcessId)
        {
            // NOTE: Process Ids are reused!
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                "SELECT * " +
                "FROM Win32_Process " +
                "WHERE ParentProcessId=" + parentProcessId);
            ManagementObjectCollection collection = searcher.Get();
            if (collection.Count > 0)
            {
                foreach (var item in collection)
                {
                    UInt32 childProcessId = (UInt32)item["ProcessId"];
                    if ((int)childProcessId != Process.GetCurrentProcess().Id)
                    {

                        KillAllProcessesSpawnedBy(childProcessId);

                        try
                        {
                            Process childProcess = Process.GetProcessById((int)childProcessId);
                            childProcess.Kill();
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }
        private void AddDataToOutput(string data)
        {
            if (_myLock == null) _myLock = new object();
            lock (_myLock) _sbOutput.AppendLine(data);
            //
            if (_updateWatch != null && _updateWatch.ElapsedMilliseconds > 1000)
            {
                UpdateOutput();
                _updateWatch.Restart();
            }
        }
        void UpdateOutput()
        {
            try
            {
                AppendOutput?.Invoke(OutputData);
                //
                if (_myLock == null) _myLock = new object();
                lock (_myLock)
                {
                    //if (Tools.WaitForFileToUnlock(_outputFileName, 5000))
                    File.AppendAllText(_outputFileName, OutputData);
                    _sbOutput.Clear();
                }
            }
            catch { }
        }
    }
}
