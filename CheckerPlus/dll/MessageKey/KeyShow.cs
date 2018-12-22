using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckerPlus.Dll.MessageKey
{
    class KeyShow
    {
        static public void Show(Keys key)
        {
#if DEBUG
            Debug.WriteLine(key);
#endif
            
            string k = key.ToString();
            foreach (var d in keys)
                if (d.key == key)
                    k = d.textkey;
            double width = System.Windows.SystemParameters.WorkArea.Width;
            double height = System.Windows.SystemParameters.WorkArea.Height;
            MessageKeyWindow window = new MessageKeyWindow()
            {
                textkey = k,
                WindowStartupLocation = System.Windows.WindowStartupLocation.Manual,
                Left = (_width - 150) / 2 + rnd.Next(-15,15),
                Top = (_height + 200) / 2 + rnd.Next(-15, 15)
            };
            window.ShowInTaskbar = false;
            window.Topmost = true;
            window.Show();
        }

        static Random rnd = new Random();
        static double _width = System.Windows.SystemParameters.WorkArea.Width;
        static double _height = System.Windows.SystemParameters.WorkArea.Height;

        /*
System.Windows.SystemParameters.WorkArea.Width
System.Windows.SystemParameters.WorkArea.Height */

        public class KT
        {
            public Keys key { get; set; }
            public string textkey { get; set; }
            public KT(Keys a,string textkey)
            {
                this.key = a;
                this.textkey = textkey;
            }
        }

        #region List
        static List<KT> keys = new List<KT>()
        {
            new KT(Keys.D1,"1"),
            new KT(Keys.D2,"2"),
            new KT(Keys.D3,"3"),
            new KT(Keys.D4,"4"),
            new KT(Keys.D5,"5"),
            new KT(Keys.D6,"6"),
            new KT(Keys.D7,"7"),
            new KT(Keys.D8,"8"),
            new KT(Keys.D9,"9"),
            new KT(Keys.D0,"0"),
            new KT(Keys.OemMinus,"-"),
            new KT(Keys.Oemplus,"+"),
            new KT(Keys.OemQuestion,"?"),
            new KT(Keys.Oemtilde,"~"),
            new KT(Keys.Oem6,"["),
            new KT(Keys.OemOpenBrackets,"["),
            new KT(Keys.Oem6,"]"),
            new KT(Keys.Oem7,"'"),
            new KT(Keys.Oem1,";"),
            new KT(Keys.OemPeriod,"."),
            new KT(Keys.Oemcomma,","),
            new KT(Keys.Return,"Enter"),
            new KT(Keys.LShiftKey,"Лев. Shift"),
            new KT(Keys.RShiftKey,"Прав. Shift"),
            new KT(Keys.Up,"Вверх"),
            new KT(Keys.Left,"Влево"),
            new KT(Keys.Right,"Вправо"),
            new KT(Keys.Down,"Вниз"),
        };

        #endregion
    }
}
