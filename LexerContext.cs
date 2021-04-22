namespace python_lexer
{
    public class LexerContext
    {
        private readonly string str;
        private int currentIndex = 0;
        
        public LexerContext(string str)
        {
            this.str = str;
        }

        public void IncIndex()
        {
            currentIndex++;
        }

        public int GetIndex()
        {
            return currentIndex;
        }
        
        public char GetCurrentChar()
        {
            return str[currentIndex];
        }
        
        public string GetCurrentCharByString()
        {
            return GetCurrentChar().ToString();
        }
        
        public char GetNextChar()
        {
            return str[currentIndex + 1];
        }
        
        public bool IsEnded()
        {
            return currentIndex >= str.Length;
        }
        
        public bool IsLast()
        {
            return currentIndex == str.Length - 1;
        }

        public string GetCharPair()
        {
            if (IsLast()) return null;
            var pair = new char[] {GetCurrentChar(), GetNextChar()};
            return new string(pair);
        }
    }
}