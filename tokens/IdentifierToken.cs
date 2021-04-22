using System;

namespace python_lexer.tokens
{
    public class IdentifierToken : IToken
    {
        private string _value;

        public IdentifierToken(string value)
        {
            beginIndex = endIndex;
            endIndex = beginIndex;
            _value = value;
        }

        public static bool IsIdentifierBegin(LexerContext context)
        {
            return context.GetCurrentChar() == '_' || Char.IsLetter(context.GetCurrentChar());
        }
    }
}