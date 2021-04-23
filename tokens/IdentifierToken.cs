using System;

namespace python_lexer.tokens
{
    public class IdentifierToken : IToken
    {
        private string _value;

        public IdentifierToken(string value, int begin, int end)
        {
            beginIndex = begin;
            endIndex = end;
            _value = value;
        }

        public static bool IsIdentifierBegin(LexerContext context)
        {
            return context.GetCurrentChar() == '_' || Char.IsLetter(context.GetCurrentChar());
        }
    }
}