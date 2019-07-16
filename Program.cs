using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Resources;

namespace EnumSetLookup
{
    class Program
    {
        public static void Main(string[] args)
        {
            
            string set = "";
            string member = "";
            string resultFile = "Resources/translatedEnums.txt";
            
            for (int i = 0; i < args.Length; i++) {
                if ((args[i].Equals("-s") || args[i].Equals("--set")) && i + 1 < args.Length) {
                    set = args[i + 1];
                    i++;
                }
                else if ((args[i].Equals("-m") || args[i].Equals("--member")) && i + 1 < args.Length) {
                    member = args[i + 1];
                    i++;
                } else if (args[i].Equals("-u") || args[i].Equals("--update")) {
                    update(resultFile, set, member);
                    return;
                } else {
                    Console.WriteLine("Unrecognized entry.");
                }
            }
            
            // Start the search function
            Console.WriteLine("Staring search for set: {0} member: {1}", set, member);
            findMatch("." + resultFile.Replace('/', '.'), set, member);  
        }

        static public void update(string file, string set, string member) {
            Console.WriteLine("Translating...");
            string newFile = "";
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name;
            using (Stream stream = assembly.GetManifestResourceStream(name + ".Resources.enums.txt")) {
                if (stream != null) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            if (line.Substring(0, 1).Equals("#")) {
                                    newFile = newFile + line + "\n";
                            } else {
                                int index = line.IndexOf('=');
                                if (index > 0) {
                                    string key = line.Substring(0, index).Trim();
                                    string value = line.Substring(index + 1).Trim();

                                    string replacementKey = replaceKeyWithValues(key);
                                    if (replacementKey.Length > 0) {
                                        newFile = newFile + replacementKey + "=" + value + "\n";
                                    } else {
                                        Console.WriteLine("ERROR. Key is nothing!");
                                    }
                                }
                            }
                        }
                    }
                } else {
                    Console.WriteLine("ERROR.");
                }
            }

            Console.WriteLine("Writing to file...");
            FileStream fs = new FileStream(file, FileMode.Create);
            using (StreamWriter sw = new StreamWriter(fs)) {
                sw.Write(newFile);
                sw.Close();
            }
        }

        ///<summary>Returns the value of a set given the file and key as a string.</summary>
        static public string findTargetByKey(string file, int key) {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name;
           
            using (Stream stream = assembly.GetManifestResourceStream(name + file)) {
                if (stream != null) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            string value = getValue(line, key);
                            if (value.Length > 0) {
                                return value;
                            }
                        }
                    }
                } else {
                    Console.WriteLine("ERROR. No file by the name: " + file);
                }
            }                
            return "";
        }

        ///<summary>Returns the value of a pair given the key matches the target.</summary>
        static public string getValue(string line, int target) {
            if (!line.Substring(0, 1).Equals("#")) {
                int index = line.IndexOf('=');
                if (index > 0) {
                    string key = line.Substring(0, index).Trim();
                    string value = line.Substring(index + 1).Trim();
                    try {
                        int keyNumber = Int32.Parse(key);
                        if (keyNumber == target) {
                            return value;
                        }
                    } catch (FormatException e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return "";
        }

        ///<summary>Returns the string replacement for a number value. Ex: 01.01 returns Color.Red</summary>
        static public string replaceKeyWithValues(string key) {
            int split = key.IndexOf('.');
            string set = key.Substring(0, split);
            string member = key.Substring(split + 1);
            int setNumber = -1;
            int memberNumber = -1;
            try {
                setNumber = Int32.Parse(set);
                memberNumber = Int32.Parse(member);
            } catch (FormatException e) {
                Console.WriteLine(e.Message);
            }

            string file = ".Resources.enumSets.txt";
            string setValue = findTargetByKey(file, setNumber);
            if (setValue != null) {
                file = ".Resources.enumSet" + setValue + ".txt";
                string memberValue = findTargetByKey(file, memberNumber);
                if (memberValue != null) {
                    return setValue + '.' + memberValue;
                }
            }
            return "";
        }

        ///<summary>Prints out all matches found in the specified file with format: Color.Red=Danger</summary>
        static public void findMatch(string file, string set, string member) {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name;
            using (Stream stream = assembly.GetManifestResourceStream(name + file)) {
                if (stream != null) {
                    using (StreamReader reader = new StreamReader(stream)) {
                        string line;
                        while ((line = reader.ReadLine()) != null) {
                            if (line.Length > 0 && !line.Substring(0, 1).Equals("#")) {
                                int index = line.IndexOf('=');
                                if (index > 0) {
                                    string key = line.Substring(0, index).Trim();
                                    string value = line.Substring(index + 1).Trim();

                                    int split = key.IndexOf('.');
                                    string tempSet = key.Substring(0, split).ToLower();
                                    string tempMember = key.Substring(split + 1).ToLower();

                                    if (((set.Length == 0) || tempSet.Equals(set.ToLower())) && 
                                        ((member.Length == 0) || tempMember.Equals(member.ToLower()))) {
                                        Console.WriteLine("Match found: {0}", line);
                                    }
                                }
                            }
                        }
                    } 
                } else {
                    Console.WriteLine("ERROR. No file by the name: " + file);
                }
            }
        }
    }
}
