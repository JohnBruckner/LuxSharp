namespace Core.Exceptions;

public class ParseError(string message) : Exception(message);