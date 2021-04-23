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
            var begin = context.GetIndex();
            if (!context.IsLast() && SpecialSymbolToken.IsSpecialPair(context))
            {
                var pair = context.GetCharPair();
                context.IncIndex();
                context.IncIndex();
                return new SpecialSymbolToken(pair, begin);
            } 
            else
            {
                var ch = context.GetCurrentChar();
                context.IncIndex();
                return new SpecialSymbolToken(ch, begin);
            }
        }
        
        private static IdentifierToken ResolveIdentifier(LexerContext context)
        {
            var builder = new StringBuilder();
            var begin = context.GetIndex();
            builder.Append(context.GetCurrentChar());
            context.IncIndex();
            while (!context.IsEnded()
                && (Char.IsDigit(context.GetCurrentChar()) 
                || Char.IsLetter(context.GetCurrentChar())))
            {
                builder.Append(context.GetCurrentChar());
                context.IncIndex();
            }

            return new IdentifierToken(builder.ToString(), begin, context.GetIndex());
        }
        
        private static NumberToken ResolveNumber(LexerContext context)
        {
            var begin = context.GetIndex();
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

            return new NumberToken(builder.ToString(), begin, context.GetIndex());
        }
        
        private static StringToken ResolveString(LexerContext context)
        {
            var begin = context.GetIndex();
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

            return new StringToken(builder.ToString(), begin, context.GetIndex());
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
            var begin = context.GetIndex();
            context.IncIndex();
            context.IncIndex();
            var inners = new List<CommentToken>();
            var isOneLine = true;
            while (!context.IsEnded())
            {
                if (CommentToken.IsTurboPascalCommentBegin(context))
                {
                    inners.Add(ResolveTurboPascalComment(context));
                    continue;
                }
                if (CommentToken.IsDelphiCommentBegin(context))
                {
                    inners.Add(ResolveDelphiComment(context));
                    continue;
                }
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

            return new CommentToken(isOneLine && inners.All(c => c.IsSingleLineComment), inners,
                                    begin, context.GetIndex());
        }
        
        private static CommentToken ResolveTurboPascalComment(LexerContext context)
        {
            var begin = context.GetIndex();
            context.IncIndex();
            var inners = new List<CommentToken>();
            var isOneLine = true;
            while (!context.IsEnded())
            {
                if (CommentToken.IsOldStyleCommentBegin(context))
                {
                    inners.Add(ResolveOldStyleComment(context));
                    continue;
                }
                if (CommentToken.IsDelphiCommentBegin(context))
                {
                    inners.Add(ResolveDelphiComment(context));
                    continue;
                }
                if (context.IsNewLineNow())
                {
                    isOneLine = false;
                    for (var i = 0; i < Environment.NewLine.Length; i++) context.IncIndex();
                }
                
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

            return new CommentToken(isOneLine && inners.All(c => c.IsSingleLineComment), inners,
                                    begin, context.GetIndex());   
        }
        
        private static CommentToken ResolveDelphiComment(LexerContext context)
        {
            var begin = context.GetIndex();
            context.IncIndex();
            context.IncIndex();
            var inners = new List<CommentToken>();
            while (!context.IsEnded() && !context.IsNewLineNow())
            {
                if (CommentToken.IsOldStyleCommentBegin(context))
                {
                    inners.Add(ResolveOldStyleComment(context));
                    continue;
                }
                if (CommentToken.IsTurboPascalCommentBegin(context))
                {
                    inners.Add(ResolveTurboPascalComment(context));
                    continue;
                }
                context.IncIndex();
            }

            var isSingleLine = inners.All(c => c.IsSingleLineComment);
            if (!isSingleLine) throw new SyntaxErrorException();
            
            return new CommentToken(true, inners, begin, context.GetIndex());
        }
        
    }
}