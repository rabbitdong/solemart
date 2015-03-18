using Solemart.SystemUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tool.Encryptor {
    class Program {
        static void Main(string[] args) {
            if (args.Length < 1){
                Console.WriteLine("usage: encryptor [-options] text");
                return;
            }
            if (args[0] == "-?" || args[0] == "/?") {
                Console.WriteLine("encrypt or decrypt a text");
                Console.WriteLine("usage: encryptor -options text");
                Console.WriteLine();
                Console.WriteLine("[-options]:");
                Console.WriteLine("   -d decrypt text");
                Console.WriteLine("   -e encrypt text");
                return;
            }

            if (args[0] == "-v" || args[0] == "/v") {
                Console.WriteLine("version: 1.0");
                return;
            }

            if ((args[0] == "-d" || args[0] == "-e") && args.Length < 2) {
                Console.WriteLine("usage: encryptor [-options] text");
                return;
            }

            if (args[0] == "-d") {
                string text = args[1];
                Console.WriteLine("{0}", EncryptUtil.DecryptString(text));
                return;
            }
            else if (args[0] == "-e") {
                string text = args[1];
                Console.WriteLine("{0}", EncryptUtil.EncryptString(text));
                return;
            }
        }
    }
}
