using System.Linq;
using NUnit.Framework;
using python_lexer.tokens;

namespace python_lexer
{
    public class TestWithoutBrackets
    {
        [Test]
        public void TestSpecialsSimple()
        {
            var lexer = new Lexer();
            var res = lexer.Run("   / + > < = -         -                *    ");
            Assert.AreEqual(8, res.Count);
            Assert.Pass();
        }
        
        [Test]
        public void TestSpecialsPairs()
        {
            var lexer = new Lexer();
            var res = lexer.Run("// + >> <<//");
            Assert.AreEqual(5, res.Count);
            Assert.Pass();
        }
        
        [Test]
        public void TestIdentifierSimple()
        {
            var lexer = new Lexer();
            var res = lexer.Run("abba + aba");
            Assert.AreEqual(3, res.Count);
            Assert.True(res[0] is IdentifierToken);
            Assert.True(res[1] is SpecialSymbolToken);
            Assert.True(res[2] is IdentifierToken);
            Assert.Pass();
        }
        
        [Test]
        public void TestIdentifier()
        {
            var lexer = new Lexer();
            var res = lexer.Run(" a b a b a b a c");
            Assert.AreEqual(8, res.Count);
            Assert.Pass();
        }
        
        [Test]
        public void TestDecimalNumber()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     123   1233  2  ");
            Assert.AreEqual(3, res.Count);
            Assert.True(res[0] is NumberToken);
            Assert.True(res[1] is NumberToken);
            Assert.True(res[2] is NumberToken);
            Assert.Pass();
        }
        
        [Test]
        public void TestDecimalNumberExpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     12354345647  ");
            Assert.True(res.Count == 1);
        }
        
        [Test]
        public void TestDecimalNumberUnxpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     123543A45647  ");
            Assert.True(res.Count > 1);
        }
        
        [Test]
        public void TestHexadecimalNumber()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     $12B3A   $C123A3  $A2B  ");
            Assert.AreEqual(3, res.Count);
            Assert.True(res[0] is NumberToken);
            Assert.True(res[1] is NumberToken);
            Assert.True(res[2] is NumberToken);
            Assert.Pass();
        }
        
        [Test]
        public void TestHexNumberExpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     $1ACBDEF12359175848745ABDF  ");
            Assert.True(res.Count == 1);
        }
        
        [Test]
        public void TestHexNumberUnexpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     $1ACBDEF12359M175848745ABDF  ");
            Assert.True(res.Count > 1);
        }
        
        [Test]
        public void TestOctalNumber()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     &1234   &777  &123  ");
            Assert.AreEqual(3, res.Count);
            Assert.True(res[0] is NumberToken);
            Assert.True(res[1] is NumberToken);
            Assert.True(res[2] is NumberToken);
            Assert.Pass();
        }
        
        [Test]
        public void TestOctalNumberExpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     &12312123  ");
            Assert.True(res.Count == 1);
        }
        
        [Test]
        public void TestOctalNumberUnexpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     &123128123  ");
            Assert.True(res.Count > 1);
        }
        
        [Test]
        public void TestBinaryNumber()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     %1001   %101010  %0  ");
            Assert.AreEqual(3, res.Count);
            Assert.True(res[0] is NumberToken);
            Assert.True(res[1] is NumberToken);
            Assert.True(res[2] is NumberToken);
            Assert.Pass();
        }
        
        [Test]
        public void TestBinaryNumberExpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     %10010101101  ");
            Assert.True(res.Count == 1);
        }
        
        [Test]
        public void TestBinaryNumberUnexpectedDigits()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     %100121111  ");
            Assert.True(res.Count > 1);
        }
        
        [Test]
        public void TestQuotedStringSimple()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     'abcd'  ");
            Assert.AreEqual(1, res.Count);
        }
        
        [Test]
        public void TestControlStringSimple()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     #12  ");
            Assert.AreEqual(1, res.Count);
        }
        
        [Test]
        public void TestQuotedControlQuotedString()
        {
            var lexer = new Lexer();
            var res = lexer.Run("     'abc'#12'def'  ");
            Assert.AreEqual(1, res.Count);
        }
        
        [Test]
        public void TestQuotedNumberQuotedNumberQuoted()
        {
            var lexer = new Lexer();
            var res = lexer.Run("    '123'321'123'321'123'  ");
            Assert.AreEqual(5, res.Count);
        }
        
        [Test]
        public void TestEscapedQuoteSimple()
        {
            var lexer = new Lexer();
            var res = lexer.Run("    ''''  ");
            Assert.AreEqual(1, res.Count);
        }
        
        [Test]
        public void TestEscapedQuoteThreeTimes()
        {
            var lexer = new Lexer();
            var res = lexer.Run("    '123''456''789'  ");
            Assert.AreEqual(1, res.Count);
        }
        
        [Test]
        public void TestEscapedQuoteAndControls()
        {
            var lexer = new Lexer();
            var res = lexer.Run("    'asd!&!@#'#11'end'  ");
            Assert.AreEqual(1, res.Count);
        }
        
        [Test]
        public void TestCombo()
        {
            var lexer = new Lexer();
            var res = lexer.Run(" aba $77 lola     << ");
            Assert.AreEqual(4, res.Count);
            Assert.True(res[0] is IdentifierToken);
            Assert.True(res[1] is NumberToken);
            Assert.True(res[2] is IdentifierToken);
            Assert.True(res[3] is SpecialSymbolToken);
        }
        
        [Test]
        public void TestCombo2()
        {
            var lexer = new Lexer();
            var res = lexer.Run(" 777'seven'#7'seven'777 ");
            Assert.AreEqual(3, res.Count);
            Assert.True(res[0] is NumberToken);
            Assert.True(res[1] is StringToken);
            Assert.True(res[2] is NumberToken);
        }
    }
}