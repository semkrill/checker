using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CheckerPlus
{
    public partial class App
    {
        private void Application_Startup(object sender, System.Windows.StartupEventArgs e)
        {
            Startup st = new Startup();
            st.Show();
        }
    }
}
