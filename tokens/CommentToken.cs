using System.Collections.Generic;

namespace python_lexer.tokens
{
    public class CommentToken : IToken
    {
        public bool IsSingleLineComment;
        public List<CommentToken> InnerComments;

        public CommentToken(bool isSingleLineComment, List<CommentToken> innerComments, int begin, int end)
        {
            IsSingleLineComment = isSingleLineComment;
            InnerComments = innerComments;
            beginIndex = begin;
            endIndex = end;
        }
        
        public static bool IsCommentBegin(LexerContext context)
        {
            return IsOldStyleCommentBegin(context)
                   || IsTurboPascalCommentBegin(context)
                   || IsDelphiCommentBegin(context);
        }
        
        public static bool IsOldStyleCommentBegin(LexerContext context)
        {
            return !context.IsEnded()
                   && !context.IsLast()
                   && context.GetCurrentChar().Equals('(')
                   && context.GetNextChar().Equals('*');
        }
        
        public static bool IsOldStyleCommentEnd(LexerContext context)
        {
            return !context.IsEnded()
                   && !context.IsLast()
                   && context.GetCurrentChar().Equals('*')
                   && context.GetNextChar().Equals(')');
        }
        
        public static bool IsTurboPascalCommentBegin(LexerContext context)
        {
            return !context.IsEnded()
                   && context.GetCurrentChar().Equals('{');
        }
        
        public static bool IsTurboPascalCommentEnd(LexerContext context)
        {
            return !context.IsEnded()
                   && context.GetCurrentChar().Equals('}');
        }
        
        public static bool IsDelphiCommentBegin(LexerContext context)
        {
            return !context.IsEnded()
                   && !context.IsLast()
                   && context.GetCurrentChar().Equals('/')
                   && context.GetNextChar().Equals('/');
        }
    }
}   