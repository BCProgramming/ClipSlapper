using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

namespace ClipSlapper
{
    class Program
    {
        [STAThread]
        private static String GetDataFormat(String input)
        {
            Dictionary<String, String> buildmapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"Text",DataFormats.Text},
                {"Unicode",DataFormats.UnicodeText},
                {"Bitmap",DataFormats.Bitmap},
                {"csv",DataFormats.CommaSeparatedValue},
                {"files",DataFormats.FileDrop},
                {"wave",DataFormats.WaveAudio}
            };
            if (buildmapping.ContainsKey(input)) return buildmapping[input];

            return input;

        }
        static void Main(string[] args)
        {

            CmdParser cp = new CmdParser();
            Dictionary<String,CmdParser.Switch> Switches  = new Dictionary<string, CmdParser.Switch>(StringComparer.OrdinalIgnoreCase);
            
            foreach(var p in cp.getSwitches())
            {
                Switches.Add(p.SwitchValue,p);
            }
            
            if (cp.hasSwitch("out"))
            {
                
                var outswitch = Switches["out"];
                String[] DesiredFormats = new string[]{DataFormats.Text,DataFormats.UnicodeText};
                if(outswitch.HasArgument())
                {
                    String[] splitstr = outswitch.Argument.Split(',');
                    DesiredFormats = (from p in splitstr select GetDataFormat(p)).ToArray();
                }

                //from clipboard to standard output.
                String retrieved = "";
                for(int i=0;i<DesiredFormats.Length;i++)
                {
                    if(Clipboard.ContainsData(DesiredFormats[i]))
                    {
                        retrieved = (String)Clipboard.GetData(DesiredFormats[i]);
                    }
                }
                
                Console.WriteLine(retrieved);
            }
            if(cp.hasSwitch("in"))
            {
                var inswitch = Switches["in"];
                //from standard input to clipboard.
                String setclip = Console.In.ReadToEnd();
                Clipboard.SetText(setclip,TextDataFormat.Text);
                
            }

            Console.ReadKey();

        }
    }
}
