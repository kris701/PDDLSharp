namespace PDDLSharp.ErrorListeners.Tests
{
    [TestClass]
    public class ErrorListenerTests
    {
        [TestMethod]
        [DataRow(ParseErrorType.None, ParseErrorType.Message)]
        [DataRow(ParseErrorType.None, ParseErrorType.Warning)]
        [DataRow(ParseErrorType.None, ParseErrorType.Error)]
        [DataRow(ParseErrorType.Message, ParseErrorType.Warning)]
        [DataRow(ParseErrorType.Message, ParseErrorType.Error)]
        [DataRow(ParseErrorType.Warning, ParseErrorType.Error)]
        [ExpectedException(typeof(ParseException))]
        public void Can_ThrowIfErrorIsAboveLimit(ParseErrorType throwLevel, ParseErrorType newErrorLevel)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(throwLevel);

            // ACT
            listener.AddError(new ParseError("", newErrorLevel, ParseErrorLevel.None));
        }

        [TestMethod]
        [DataRow(ParseErrorType.None, ParseErrorType.None)]
        [DataRow(ParseErrorType.Message, ParseErrorType.None)]
        [DataRow(ParseErrorType.Message, ParseErrorType.Message)]
        [DataRow(ParseErrorType.Warning, ParseErrorType.None)]
        [DataRow(ParseErrorType.Warning, ParseErrorType.Message)]
        [DataRow(ParseErrorType.Warning, ParseErrorType.Warning)]
        [DataRow(ParseErrorType.Error, ParseErrorType.Warning)]
        [DataRow(ParseErrorType.Error, ParseErrorType.Error)]
        public void Cant_ThrowIfErrorIsBelowOrOnLimit(ParseErrorType throwLevel, ParseErrorType newErrorLevel)
        {
            // ARRANGE
            IErrorListener listener = new ErrorListener(throwLevel);

            // ACT
            listener.AddError(new ParseError("", newErrorLevel, ParseErrorLevel.None));
        }
    }
}