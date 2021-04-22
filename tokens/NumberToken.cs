using System;
using System.Collections.Generic;

namespace python_lexer.tokens
{
    public class NumberToken: IToken
    {
        private string _value;

        private static readonly HashSet<char> HexadecimalDigits = new HashSet<char> {'0', '1', '2', '3', '4',
            '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};
        private static readonly HashSet<char> OctalDigits = new HashSet<char> {'0', '1', '2', '3', '4',
            '5', '6', '7'};
        private static readonly HashSet<char> BinaryDigits = new HashSet<char> {'0', '1'};

        public NumberToken(string value)
        {
            _value = value;
        }
        
        public static bool IsDecimalNumberBegin(LexerContext context)
        {
            return Char.IsDigit(context.GetCurrentChar());
        }
        
        public static bool IsHexNumberBegin(LexerContext context)
        {
            return context.GetCurrentChar().Equals('$')
                   && !context.IsEnded()
                   && !context.IsLast()
                   && HexadecimalDigits.Contains(context.GetNextChar());
        }
        
        public static bool IsOctalNumberBegin(LexerContext context)
        {
            return context.GetCurrentChar().Equals('&')
                   && !context.IsEnded()
                   && !context.IsLast()
                   && OctalDigits.Contains(context.GetNextChar());
        }
        
        public static bool IsBinaryNumberBegin(LexerContext context)
        {
            return context.GetCurrentChar().Equals('%')
                   && !context.IsEnded()
                   && !context.IsLast()
                   && HexadecimalDigits.Contains(context.GetNextChar());
        }
        
        public static bool IsNumberBegin(LexerContext context)
        {
            return IsDecimalNumberBegin(context)
                   || IsHexNumberBegin(context)
                   || IsOctalNumberBegin(context)
                   || IsBinaryNumberBegin(context);
        }
        
        public static bool IsDecimalDigit(LexerContext context)
        {
            return Char.IsDigit(context.GetCurrentChar());
        }
        
        public static bool IsHexadecimalDigit(LexerContext context)
        {
            return HexadecimalDigits.Contains(context.GetCurrentChar());
        }
        
        public static bool IsOctalDigit(LexerContext context)
        {
            return OctalDigits.Contains(context.GetCurrentChar());
        }
        
        public static bool IsBinaryDigit(LexerContext context)
        {
            return BinaryDigits.Contains(context.GetCurrentChar());
        }
    }
}