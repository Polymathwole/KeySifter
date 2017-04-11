using System;
using System.IO;

namespace EncryptionKeySifter
{
    public class TraverseEventArgs : EventArgs
    {
        public int total;

        public TraverseEventArgs(int t)
        {
            total = t;
        }
    }

    class Program
    {
        public event EventHandler<TraverseEventArgs> TraverseFinished;

        protected virtual void OnTraverseFinished(TraverseEventArgs e)
        {
            if (TraverseFinished != null)
                TraverseFinished(this, e);
        }

        public static string getNow
        {
            get
            {
                string xxx = DateTime.Now.ToString();
                return xxx.Replace("-", "").Replace(":", "").Replace("\\", "").Replace("/", "");
            }
        }

        public static string processString(string val)  //final finishing!!!
        {
            string val1 = val.Substring(0, 15) + val.Substring(15, 20).Replace(" ", "");
            string val2 = val.Substring(35, 19).Replace(" ", "");

            return val1 + "\t\t" + val2;
        }

        public void siftKeys()
        {
            int count = 0;
            int total = 0;

            String msg = "\nThis encryption key sifter application is written by Oluwaremi Oluwole, SN027053, of ATM Support. It is not to be distributed without the owner's express permission.\n";

            Console.WriteLine(msg);
            Console.Write("Enter file directory: ");
            string sss = Console.ReadLine().Replace("\"", "");//to remove quotes when path is copied

            Console.WriteLine("\n ");

            string path = "Final encryption keys " + getNow + ".txt";

            try
            {
                using (StreamReader sr = File.OpenText(sss))
                using (StreamWriter sw = File.CreateText(path))
                {
                    FileInfo fi = new FileInfo(path);
                    fi.IsReadOnly = true;

                    while (sr.Peek() > -1)
                    {
                        string ss = sr.ReadLine();
                        string sst = ss.Trim();

                        if (sst.StartsWith("Online-AUTH>") || sst.StartsWith("Encrypted ZMK component") || sst.StartsWith("Input") || sst.StartsWith("Enter") || sst.StartsWith("Data") || sst.StartsWith("Terminated") || sst.StartsWith("Zero") || sst.StartsWith("WARNING") || sst.StartsWith("Function") || String.IsNullOrEmpty(sst) || (sst.Length < 35))
                            continue;

                        ++count;

                        if (sst.StartsWith("Clear ZMK component:"))
                        {
                            if (count == 1)
                                sw.WriteLine("Key {0}", total + 1);

                            sst = sst.Substring(21);
                            sw.Write(sst + "\t\tCHK: ");
                        }
                        else if (sst.StartsWith("Key check value:"))
                        {
                            if (count == 6)
                            {
                                sst = sst.Substring(0, 24);
                                String sst1 = sst.Substring(0, 17);
                                String sst2 = sst.Substring(17, 7);
                                sw.WriteLine(sst1 + sst2.Replace(" ", ""));
                                sw.WriteLine();
                                sw.WriteLine();
                                sw.WriteLine();
                            }
                            else
                            {
                                sst = sst.Substring(17);
                                sw.WriteLine(sst.Substring(0, 4));
                            }
                        }
                        else if (sst.StartsWith("Encrypted ZMK"))
                        {
                            if (count != 5) //You party pooper. Try this on for size!!!
                                count = 5;
                            sw.WriteLine(processString(sst));
                        }

                        if (count == 6)
                        {
                            count = 0;
                            ++total;
                        }
                    }

                    OnTraverseFinished(new TraverseEventArgs(total));
                }
            }
            catch (FileNotFoundException ff)
            {
                Console.WriteLine(ff.Message);
                return;
            }
        }

        public static void Trav(object o, TraverseEventArgs tve)
        {
            Console.WriteLine("Done! {0}" + " encryption keys successfully sifted!\n", tve.total);
        }

        public static void Main()
        {
            Program ecs = new Program();
            ecs.TraverseFinished += Trav;
            ecs.siftKeys();
        }
    }
}
