using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HackAssembler.Model;
using Microsoft.Win32;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace HackAssembler.ViewModel
{
    public class MainViewModel : ViewModelBase, INotifyPropertyChanged
    {
        #region Properties

        private ObservableCollection<Symbol> SymbolTable { get; set; }
        private ObservableCollection<Symbol> JumpTable { get; set; }
        private ObservableCollection<Symbol> DestTable { get; set; }
        private ObservableCollection<Symbol> CompTable { get; set; }

        public string Value { get; set; }
        public string SymbolName { get; set; }
        public string Code { get; set; }
        public string Error_Msg { get; set; }
        public int Error_Counter { get; set; }
        public string BinaryCode { get; set; }
        public string AsmFileName { get; set; }
        public string AsmFilePath { get; set; }
        public bool ConstDescOpen { get; set; }
        #endregion

        public ICommand AssembleCmd { get; private set; }
        public ICommand LoadFileCmd { get; private set; }
        public ICommand SaveFileCmd { get; private set; }
        public ICommand OpenConstDescCmd { get; private set; }

        public MainViewModel()
        {
            // Initialize commands
            AssembleCmd = new DelegateCommand<string>(assamble_exec, assamble_validate);
            LoadFileCmd = new DelegateCommand<string>(load_file_exec, load_file_validate);
            SaveFileCmd = new DelegateCommand<string>(save_file_exec, save_file_validate);
            this.OpenConstDescCmd = new ActionCommand<bool>(OpenConstDesc);
                
            // Set default value for FileName, Error Message and set the error counter to zero
            AsmFileName = "NewAsm";
            Error_Msg = String.Empty;
            Error_Counter = 0;  
        }

        #region Methods
        
        public void load_all_Tables()
        {
            // SymbolTable
            SymbolTable.Add(new Symbol("R0", Convert.ToString(0)));
            SymbolTable.Add(new Symbol("R1", Convert.ToString(1)));
            SymbolTable.Add(new Symbol("R2", Convert.ToString(2)));
            SymbolTable.Add(new Symbol("R3", Convert.ToString(3)));
            SymbolTable.Add(new Symbol("R4", Convert.ToString(4)));
            SymbolTable.Add(new Symbol("R5", Convert.ToString(5)));
            SymbolTable.Add(new Symbol("R6", Convert.ToString(6)));
            SymbolTable.Add(new Symbol("R7", Convert.ToString(7)));
            SymbolTable.Add(new Symbol("R8", Convert.ToString(8)));
            SymbolTable.Add(new Symbol("R9", Convert.ToString(9)));
            SymbolTable.Add(new Symbol("R10", Convert.ToString(10)));
            SymbolTable.Add(new Symbol("R11", Convert.ToString(11)));
            SymbolTable.Add(new Symbol("R12", Convert.ToString(12)));
            SymbolTable.Add(new Symbol("R13", Convert.ToString(13)));
            SymbolTable.Add(new Symbol("R14", Convert.ToString(14)));
            SymbolTable.Add(new Symbol("R15", Convert.ToString(15)));
            SymbolTable.Add(new Symbol("SCREEN", Convert.ToString(16384)));
            SymbolTable.Add(new Symbol("KBD", Convert.ToString(24576)));
            // Destination Parts
            DestTable.Add(new Symbol(null, "000"));
            DestTable.Add(new Symbol("M", "001"));
            DestTable.Add(new Symbol("D", "010"));
            DestTable.Add(new Symbol("MD", "011"));
            DestTable.Add(new Symbol("A", "100"));
            DestTable.Add(new Symbol("AM", "101"));
            DestTable.Add(new Symbol("AD", "110"));
            DestTable.Add(new Symbol("AMD", "111"));
            // Jump Parts
            JumpTable.Add(new Symbol(null, "000"));
            JumpTable.Add(new Symbol("JGT", "001"));
            JumpTable.Add(new Symbol("JEQ", "010"));
            JumpTable.Add(new Symbol("JGE", "011"));
            JumpTable.Add(new Symbol("JLT", "100"));
            JumpTable.Add(new Symbol("JNE", "101"));
            JumpTable.Add(new Symbol("JLE", "110"));
            JumpTable.Add(new Symbol("JMP", "111"));
            // Computation Parts
            CompTable.Add(new Symbol("0", "10101010"));
            CompTable.Add(new Symbol("1", "10111111"));
            CompTable.Add(new Symbol("-1", "10111010"));
            CompTable.Add(new Symbol("D", "10001100"));
            CompTable.Add(new Symbol("A", "10110000"));
            CompTable.Add(new Symbol("!D", "10001101"));
            CompTable.Add(new Symbol("!A", "10110001"));
            CompTable.Add(new Symbol("-D", "10001111"));
            CompTable.Add(new Symbol("-A", "10110011"));
            CompTable.Add(new Symbol("D+1", "10011111"));
            CompTable.Add(new Symbol("A+1", "10110111"));
            CompTable.Add(new Symbol("D-1", "10001110"));
            CompTable.Add(new Symbol("A-1", "10110010"));
            CompTable.Add(new Symbol("D+A", "10000010"));
            CompTable.Add(new Symbol("D-A", "10010011"));
            CompTable.Add(new Symbol("A-D", "10000111"));
            CompTable.Add(new Symbol("D&A", "10000000"));
            CompTable.Add(new Symbol("D|A", "10010101"));
            CompTable.Add(new Symbol("M", "11110000"));
            CompTable.Add(new Symbol("!M", "11110001"));
            CompTable.Add(new Symbol("-M", "11110011"));
            CompTable.Add(new Symbol("M+1", "11110111"));
            CompTable.Add(new Symbol("M-1", "11110010"));
            CompTable.Add(new Symbol("D+M", "11000010"));
            CompTable.Add(new Symbol("D-M", "11010011"));
            CompTable.Add(new Symbol("M-D", "11000111"));
            CompTable.Add(new Symbol("D&M", "11000000"));
            CompTable.Add(new Symbol("D|M", "11010101"));
            // New computations from our ALU
            CompTable.Add(new Symbol("M*D", "00000010"));
            CompTable.Add(new Symbol("M/D", "00000011"));
            CompTable.Add(new Symbol("M+D^2", "00010000"));
            CompTable.Add(new Symbol("M-D^2", "00010001"));
            CompTable.Add(new Symbol("M*D^2", "00010010"));
            CompTable.Add(new Symbol("M/D^2", "00010011"));
            CompTable.Add(new Symbol("-M+D", "00100000"));
            CompTable.Add(new Symbol("-M-D", "00100001"));
            CompTable.Add(new Symbol("-M*D", "00100010"));
            CompTable.Add(new Symbol("-M/D", "00100011"));
            CompTable.Add(new Symbol("-M+D^2", "00110000"));
            CompTable.Add(new Symbol("-M-D^2", "00110001"));
            CompTable.Add(new Symbol("-M*D^2", "00110010"));
            CompTable.Add(new Symbol("-M/D^2", "00110011"));
            CompTable.Add(new Symbol("M^2+D", "01000000"));
            CompTable.Add(new Symbol("M^2-D", "01000001"));
            CompTable.Add(new Symbol("M^2*D", "01000010"));
            CompTable.Add(new Symbol("M^2/D", "01000011"));
            CompTable.Add(new Symbol("M^2+D^2", "01010000"));
            CompTable.Add(new Symbol("M^2-D^2", "01010001"));
            CompTable.Add(new Symbol("M^2*D^2", "01010010"));
            CompTable.Add(new Symbol("M^2/D^2", "01010011"));
            CompTable.Add(new Symbol("-M^2+D", "01100000"));
            CompTable.Add(new Symbol("-M^2-D", "01100001"));
            CompTable.Add(new Symbol("-M^2*D", "01100010"));
            CompTable.Add(new Symbol("-M^2/D", "01100011"));
            CompTable.Add(new Symbol("-M^2+D^2", "01110000"));
            CompTable.Add(new Symbol("-M^2-D^2", "01110001"));
            CompTable.Add(new Symbol("-M^2*D^2", "01110010"));
            CompTable.Add(new Symbol("-M^2/D^2", "01110011"));
            CompTable.Add(new Symbol("M^2", "00000100"));
            CompTable.Add(new Symbol("D^2", "00000101"));
        }

        private void load_file_exec(string filePath)
        {
            var Dialog = new OpenFileDialog();
            if (Dialog.ShowDialog() == true)
            {
                AsmFilePath = Dialog.FileName;
                AsmFileName = Path.GetFileNameWithoutExtension(AsmFilePath);
                using (var sr = new StreamReader(AsmFilePath))
                {
                    Code = sr.ReadToEnd();
                }
            }          
        }
        private bool load_file_validate(string filePath)
        {
            return true;
        }

        private void save_file_exec(string filePath)
        {
            var Dialog = new SaveFileDialog();
            Dialog.FileName = AsmFileName;
            Dialog.DefaultExt = ".asm";
            Dialog.Filter = "Text documents (.asm)|*.asm";
            if (Dialog.ShowDialog() == true)
            {
                File.WriteAllText(Dialog.FileName, Code);
            }
        }
        private bool save_file_validate(string filePath)
        {
            return true;
        }

        private void assamble_exec(string Code)
        {
            int next_address = 16;
            string line;
            string preOut;
            int line_number = 0;
            int mod_line_number;
            int counter_parsed_labels = 0;
            string OutputFile;
            string string_value;
            string inst_output;
            string output;
            StringBuilder sb = new StringBuilder();
            Error_Msg = String.Empty;
            Error_Counter = 0;
            string[] ErrorLines = new string[15]; // capturing 15 errors and if there are more: 15+ for output

            // Initialize all Tables and load the with predefined values
            SymbolTable = new ObservableCollection<Symbol>();
            DestTable = new ObservableCollection<Symbol>();
            JumpTable = new ObservableCollection<Symbol>();
            CompTable = new ObservableCollection<Symbol>();
            load_all_Tables();     

            // Read the input code
            OutputFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ConvertFiles\" + AsmFileName + ".hack");

            // if output file already exists, make it blank
            if (File.Exists(OutputFile))
                File.Create(OutputFile).Dispose();

            StringReader file = new StringReader(Code);
            try
            {
                // 1st run to take all labels and put them into SymbolTable and afterwards delete them from the text
                while ((line = file.ReadLine()) != null)
                {
                    string trimmed_line = line.Trim();
                    if ((trimmed_line.IndexOf('/')) != 0 && (trimmed_line.Length != 0))
                    {
                        // count output line numbers for labels to match them directly at first run
                        line_number++;
                        if (trimmed_line[0] == '(')
                        {
                            string_value = parse_after_instruction_symbol(trimmed_line);
                            mod_line_number = line_number - counter_parsed_labels - 1; // -1 is adaption for countering beginning at 0
                            parse_label(string_value, next_address, mod_line_number);
                            counter_parsed_labels++;
                            next_address++;
                        }
                        else
                        {
                            sb.AppendLine(trimmed_line);
                        }
                    }
                }
                // 2nd run after labels were taken
                line_number = 0;             
                preOut = sb.ToString().Trim();
                sb = new StringBuilder();
                file = new StringReader(preOut);
                while ((line = file.ReadLine()) != null)
                {
                    line_number++;
                    string_value = parse_after_instruction_symbol(line);
                    if (line[0] == '@')
                    {
                        bool isNumeric = UInt16.TryParse(string_value, out UInt16 n);
                        // case sensitive, if it is a number for A-instruction or a variable
                        if (isNumeric == true)
                        {
                            inst_output = parse_A_Instruction(string_value);
                        }
                        else
                        {
                            inst_output = parse_variable(string_value, next_address);
                            if (Convert.ToUInt16(inst_output) == next_address)
                                next_address++;
                            inst_output = parse_A_Instruction(inst_output);
                        }
                    }
                    else
                    {
                        inst_output = parse_c_instruction(string_value);                                             
                    }
          
                    if (inst_output == "1100000000000000" || inst_output.Length != 16)
                    {
                        ErrorLines[Error_Counter] = string_value;
                        Error_Counter++;
                    }
                    else
                        sb.AppendLine(inst_output);
                }

                if(Error_Counter != 0)
                {
                    if (Error_Counter > 15)
                        Error_Msg = "More than 15 Errors detected";
                    else
                    {
                        Error_Msg = Error_Counter.ToString() + " error(s) detected: ";
                        for (int i = 0; i < Error_Counter; i++)
                        {
                            Error_Msg = Error_Msg + ErrorLines[i];
                            if (Error_Counter > 1)
                                Error_Msg += ", ";
                        }
                    }

                    BinaryCode = String.Empty;
                    string messageBoxText = "Input code is wrong and couldn't get assembled correctly";
                    string caption = "Error";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }
                else
                {
                    output = sb.ToString().Trim();
                    BinaryCode = output;

                    var Dialog = new SaveFileDialog();
                    Dialog.FileName = AsmFileName;
                    Dialog.DefaultExt = ".hack";
                    Dialog.Filter = "Text documents (.hack)|*.hack";
                    if (Dialog.ShowDialog() == true)
                    {
                        File.WriteAllText(Dialog.FileName, output);
                    }

                    string messageBoxText = "Assembling completed! Binary output is shown at the right side";
                    string caption = "Status";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Information;
                    MessageBox.Show(messageBoxText, caption, button, icon);
                }
                
            }
            catch
            {
                string messageBoxText = "Input code is wrong and couldn't get assembled";
                string caption = "Error";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Error;
                MessageBox.Show(messageBoxText, caption, button, icon);
            }          
        }
        private bool assamble_validate(string Code)
        {
            return true;
        }
        public string parse_after_instruction_symbol(string trimmed_line)
        {
            int inst_length = 0;
            string comment_compare = String.Empty;
            char[] singleChars =  new char[2];
            char[] instruction = new char[50];

            trimmed_line = trimmed_line.Replace(" ", "");

            if (trimmed_line[0] != '@' && trimmed_line[0] != '(')
            {
                for (int i = 0; i < trimmed_line.Length; i++)
                {
                    if( i < trimmed_line.Length - 1)
                    {
                        singleChars[0] = trimmed_line[i];
                        singleChars[1] = trimmed_line[i + 1];
                        comment_compare = new string(singleChars);
                    }
                        
                    if ((trimmed_line[i] != ' ') && (trimmed_line[i] != ')') && (comment_compare != "//") && (trimmed_line[i] != '\t'))
                    {
                        instruction[i] = trimmed_line[i];
                        inst_length++;
                    }
                    else
                        break;
                }
            }
            else
            {
                for (int i = 1; i < trimmed_line.Length; i++)
                {
                    if ((trimmed_line[i] != ' ') && (trimmed_line[i] != ')') && (trimmed_line[i] != '/') && (trimmed_line[i] != '\\'))
                    {
                        instruction[i - 1] = trimmed_line[i];
                        inst_length++;
                    }
                    else
                        break;
                }
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < inst_length; i++)
            {
                sb.Append(instruction[i]);
            }
            // string_value: string of calculation or after leading indicating symbol '@' or '(' for a_instruction or label
            string string_value = sb.ToString();
            return string_value;
        }
        public string parse_c_instruction(string instruction)
        {
            string c_instruction_bin;
            string dest = "000";
            string comp = "00000000";
            string jump = "000";
            Symbol Match;
            int index;
            index = instruction.IndexOf('=');
            if (index > 0)
            {
                jump = "000";
                dest = instruction.Substring(0, index);
                Match = DestTable.FirstOrDefault(i => i.Name == dest);
                if (Match != null)
                    dest = Match.Value;
                comp = instruction.Substring(index + 1, instruction.Length - index - 1);
                Match = CompTable.FirstOrDefault(i => i.Name == comp);
                if (Match != null)
                    comp = Match.Value;
            }
            index = instruction.IndexOf(';');
            if (index > 0)
            {
                dest = "000";
                comp = instruction.Substring(0, index);
                Match = CompTable.FirstOrDefault(i => i.Name == comp);
                if (Match != null)
                    comp = Match.Value;
                jump = instruction.Substring(index + 1, instruction.Length - index - 1);
                Match = JumpTable.FirstOrDefault(i => i.Name == jump);
                if (Match != null)
                    jump = Match.Value;
            }
            c_instruction_bin = "11" + comp + dest + jump;
            return c_instruction_bin;
        }
        public string parse_A_Instruction(string input)
        {
            UInt16 Dezimalwert = UInt16.Parse(input);
            string output = Convert.ToString(Dezimalwert, 2);
            int diff_to_fill;
            diff_to_fill = 15 - output.Length;
            for (int i = 0; i < diff_to_fill; i++)
            {
                output = "0" + output;
            }
            output = "0" + output;
            return output;  // 16-Bit A-Instruction with leading 0 to indicate
        }
        public string parse_variable(string input, int next_address)
        {
            string output;
            // Check if variable is already stored
            Symbol firstMatch = SymbolTable.FirstOrDefault(i => i.Name == input);
            if (firstMatch != null)
            {
                output = Convert.ToString(firstMatch.Value);
            }
            else
            {
                // there is no variable with given name in the SymbolTable, so we have to store it and return 
                // the assigned address value in SymbolTable
                SymbolTable.Add(new Symbol(input, Convert.ToString(next_address)));
                output = Convert.ToString(next_address);
            }
            return output;
        }
        public void parse_label(string input, int next_address, int line_number)
        {
            SymbolTable.Add(new Symbol(input, Convert.ToString(line_number)));
        }
        private void OpenConstDesc(bool open) { this.ConstDescOpen = open; }
    }
    public class ActionCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private Action<T> _action;

        public ActionCommand(Action<T> action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter) { return true; }

        public void Execute(object parameter)
        {
            if (_action != null)
            {
                var castParameter = (T)Convert.ChangeType(parameter, typeof(T));
                _action(castParameter);
            }
        }
    }

    #endregion
}