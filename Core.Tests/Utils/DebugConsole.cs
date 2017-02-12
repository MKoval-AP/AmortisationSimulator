using System;
using System.IO;
using System.Text;

namespace AmortisationSimulator.Core.Tests.Utils
{
    class DebugConsole : TextWriter
    {
        public const int HeaderLength = 80;

        readonly TextWriter _oldConsole;
        bool _doIndent;
        private int _indent;

        public DebugConsole()
        {
            _doIndent = true;
            _oldConsole = Console.Out;
            Console.SetOut(this);
        }

        public void Indent() => _indent++;
        public void Unindent() => _indent--;

        public void WriteHeader(string message) => _oldConsole.WriteLine(CenterAndFill(message.ToUpper(), '=', HeaderLength));
        public void WriteFooter(string message) => _oldConsole.WriteLine(CenterAndFill(message, '-', HeaderLength));

        private static string CenterAndFill(string message, char filler, int length)
        {
            if (string.IsNullOrEmpty(message))
            {
                return new string(filler, length);
            }

            if (message.Length >= length)
            {
                return message;
            }

            var t = length - message.Length;
            return $"{new string(filler, t / 2)}{message}{new string(filler, t - t / 2)}";
        }

        public override void Write(char ch)
        {
            if (_doIndent)
            {
                _doIndent = false;
                for (var ix = 0; ix < _indent; ++ix)
                {
                    _oldConsole.Write("\t");
                }
            }

            _oldConsole.Write(ch);
            if (ch == '\n')
            {
                _doIndent = true;
            }
        }

        public override Encoding Encoding => _oldConsole.Encoding;
    }
}