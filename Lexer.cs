using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using python_lexer.tokens;

namespace python_lexer
{
    public class Lexer
    {
        public List<IToken> Run(string str)
        {
            var context = new LexerContext(str);
            var res = new List<IToken>();
            ResolveWhitespace(context);
            while (!context.IsEnded())
            {
                res.Add(Resolve(context));
                ResolveWhitespace(context);
            }
            return res;
        }

        private static IToken Resolve(LexerContext context)
        {
            if (NumberToken.IsNumberBegin(context))
            {
                return ResolveNumber(context);
            }
            else if (SpecialSymbolToken.IsSpecial(context))
            {
                return ResolveSpecial(context);
            }
            else if (IdentifierToken.IsIdentifierBegin(context))
            {
                return ResolveIdentifier(context);
            }
            else
            {
                throw new SyntaxErrorException();
            }
        }
        
        private static void ResolveWhitespace(LexerContext context)
        {
            while (!context.IsEnded() && (context.GetCurrentChar().Equals(' ') || ResolveNewLine(context)))
            {
                context.IncIndex();                
            }
        }

        private static bool ResolveNewLine(LexerContext context)
        {
            var stringSep = Environment.NewLine;
            if (stringSep.Length == 1 && stringSep.Equals(context.GetCurrentCharByString()))
            {
                return true;
            }
            else if (stringSep.Length == 2 && !context.IsLast() && stringSep.Equals(context.GetCharPair()))
            {
                context.IncIndex();
                return true;
            }
            return false;
        }

        private static SpecialSymbolToken ResolveSpecial(LexerContext context)
        {
            if (!context.IsLast() && SpecialSymbolToken.IsSpecialPair(context))
            {
                var pair = context.GetCharPair();
                context.IncIndex();
                context.IncIndex();
                return new SpecialSymbolToken(pair);
            } 
            else
            {
                var ch = context.GetCurrentChar();
                context.IncIndex();
                return new SpecialSymbolToken(ch);
            }
        }
        
        private static IdentifierToken ResolveIdentifier(LexerContext context)
        {
            var builder = new StringBuilder();
            builder.Append(context.GetCurrentChar());
            context.IncIndex();
            while (!context.IsEnded()
                && (Char.IsDigit(context.GetCurrentChar()) 
                || Char.IsLetter(context.GetCurrentChar())))
            {
                builder.Append(context.GetCurrentChar());
                context.IncIndex();
            }

            return new IdentifierToken(builder.ToString());
        }
        
        private static NumberToken ResolveNumber(LexerContext context)
        {
            var builder = new StringBuilder();
            if (NumberToken.IsDecimalNumberBegin(context))
            {
                while (!context.IsEnded() && NumberToken.IsDecimalDigit(context))
                {
                    builder.Append(context.GetCurrentChar());
                    context.IncIndex();
                }
            }
            else if (NumberToken.IsHexNumberBegin(context))
            {
                context.IncIndex();
                while (!context.IsEnded() && NumberToken.IsHexadecimalDigit(context))
                {
                    builder.Append(context.GetCurrentChar());
                    context.IncIndex();
                }
            }
            else if (NumberToken.IsOctalNumberBegin(context))
            {
                context.IncIndex();
                while (!context.IsEnded() && NumberToken.IsOctalDigit(context))
                {
                    builder.Append(context.GetCurrentChar());
                    context.IncIndex();
                }
            }
            else
            {
                context.IncIndex();
                while (!context.IsEnded() && NumberToken.IsBinaryDigit(context))
                {
                    builder.Append(context.GetCurrentChar());
                    context.IncIndex();
                }
            }

            return new NumberToken(builder.ToString());
        }
    }
}