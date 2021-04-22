using System;
using System.Collections.Generic;

namespace python_lexer.tokens
{
    public class SpecialSymbolToken: IToken
    {
        private static readonly HashSet<char> SpecialCharacters = new HashSet<char> {'’', '+', '-', '*', '/',
            '=', '<', '>', '[', ']', '.', ',', '(', ')', ':', '^', '@', '{', '}', '$', '#', '&', '%'};
        private static readonly HashSet<string> SpecialPairs = new HashSet<string> {"<<", ">>", "**", "<>", "><",
            "<=", ">=", ":=", "+=", "-=", "*=", "/=", "(*", "*)", "(.", ".)", "//"};

        private string _specials;

        public SpecialSymbolToken(string specials)
        {
            this._specials = specials;
        }
        
        public SpecialSymbolToken(char special)
        {
            this._specials = special.ToString();
        }

        public static bool IsSpecial(LexerContext context)
        {
            return SpecialCharacters.Contains(context.GetCurrentChar());
        }
        
        public static bool IsSpecialPair(LexerContext context)
        {
            return SpecialPairs.Contains(context.GetCharPair());
        }
    }
}