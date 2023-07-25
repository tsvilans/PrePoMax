using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CaeJob;


namespace PrePoMax.Forms
{
    class EnvVarsUIEditor : UITypeEditor
    {
        // Overrides                                                                                                                
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var svc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (svc != null)
            {
                using (var frm = new FrmGetEnvVars((List<EnvironmentVariable>)value))
                {
                    if (svc.ShowDialog(frm) == DialogResult.OK)
                    {
                        value = frm.EnvironmentVariables;
                    }
                }
            }
            return value;
        }
    }
}
