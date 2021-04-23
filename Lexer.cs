using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
            if (CommentToken.IsCommentBegin(context))
            {
                return ResolveComment(context);
            }
            else if (NumberToken.IsNumberBegin(context))
            {
                return ResolveNumber(context);
            }
            else if (StringToken.IsStringBegin(context))
            {
                return ResolveString(context);
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
        
        private static StringToken ResolveString(LexerContext context)
        {
            var builder = new StringBuilder();
            while (StringToken.IsStringBegin(context))
            {
                if (context.GetCurrentChar().Equals('\''))
                {
                    ResolveQuotedString(context, builder);
                } 
                else
                {
                    ResolveControlString(context, builder);
                }   
            }

            return new StringToken(builder.ToString());
        }
        
        private static void ResolveQuotedString(LexerContext context, StringBuilder builder)
        {
            context.IncIndex();

            while (!context.IsEnded() && StringToken.IsStringSymbol(context.GetCurrentChar()))
            {
                if (context.GetCurrentChar().Equals('\''))
                {
                    if (StringToken.IsDoubleQuote(context))
                    {
                        builder.Append('\'');
                        context.IncIndex();
                        context.IncIndex();
                    }
                    else
                    {
                        context.IncIndex();
                        return;
                    }
                }
                builder.Append(context.GetCurrentChar());
                context.IncIndex();
            }
        }
        
        private static void ResolveControlString(LexerContext context, StringBuilder builder)
        {
            context.IncIndex();

            var numberBuilder = new StringBuilder();
            while (!context.IsEnded() && Char.IsDigit(context.GetCurrentChar()))
            {
                numberBuilder.Append(context.GetCurrentChar());
                context.IncIndex();
            }

            var num = Int32.Parse(numberBuilder.ToString());
            builder.Append((char) num);
        }
        
        private static CommentToken ResolveComment(LexerContext context)
        {
            if (CommentToken.IsOldStyleCommentBegin(context))
            {
                return ResolveOldStyleComment(context);
            }
            else if (CommentToken.IsTurboPascalCommentBegin(context))
            {
                return ResolveTurboPascalComment(context);
            }
            else
            {
                return ResolveDelphiComment(context);
            }
        }
        
        private static CommentToken ResolveOldStyleComment(LexerContext context)
        {
            context.IncIndex();
            context.IncIndex();
            var inners = new List<CommentToken>();
            var isOneLine = true;
            while (!context.IsEnded())
            {
                if (context.IsNewLineNow())
                {
                    isOneLine = false;
                    for (var i = 0; i < Environment.NewLine.Length; i++) context.IncIndex();
                }

                if (CommentToken.IsOldStyleCommentEnd(context))
                {
                    context.IncIndex();
                    context.IncIndex();
                    break;
                }
                context.IncIndex();
            }

            return new CommentToken(isOneLine && inners.All(c => c.IsSingleLineComment), inners);    
        }
        
        private static CommentToken ResolveTurboPascalComment(LexerContext context)
        {
            context.IncIndex();
            var inners = new List<CommentToken>();
            var isOneLine = true;
            while (!context.IsEnded())
            {
                if (context.IsNewLineNow())
                {
                    isOneLine = false;
                    for (var i = 0; i < Environment.NewLine.Length; i++) context.IncIndex();
                }

                if (CommentToken.IsTurboPascalCommentEnd(context))
                {
                    context.IncIndex();
                    break;
                }
                context.IncIndex();
            }

            return new CommentToken(isOneLine && inners.All(c => c.IsSingleLineComment), inners);   
        }
        
        private static CommentToken ResolveDelphiComment(LexerContext context)
        {
            var inners = new List<CommentToken>();
            while (!context.IsEnded() && !context.IsNewLineNow())
            {
                context.IncIndex();
            }

            return new CommentToken(inners.All(c => c.IsSingleLineComment), inners);
        }
        
    }
}