using System;

namespace python_lexer.tokens
{
    public class StringToken : IToken
    {
        private string _value;

        public StringToken(string value, int begin, int end)
        {
            _value = value;
            beginIndex = begin;
            endIndex = end;
        }

        public static bool IsStringBegin(LexerContext context)
        {
            return !context.IsEnded()
                   && !context.IsLast()
                   && (
                       context.GetCurrentChar().Equals('\'') && IsStringSymbol(context.GetNextChar())
                       || context.GetCurrentChar().Equals('#') && Char.IsDigit(context.GetNextChar())
                   );
        }

        public static bool IsStringSymbol(char symbol)
        {
            return symbol < 128;
        }
        
        public static bool IsDoubleQuote(LexerContext context)
        {
            return !context.IsEnded()
                   && !context.IsLast()
                   && context.GetCurrentChar().Equals('\'')
                   && context.GetNextChar().Equals('\'');

        }
    }
}