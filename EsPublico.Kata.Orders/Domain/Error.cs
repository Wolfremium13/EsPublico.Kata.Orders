namespace EsPublico.Kata.Orders.Domain;

public record Error(string Message);

public record InvalidParameter(string Message) : Error(Message);

public record MissingParameter(string Message) : Error(Message);

public record NotFound(string Message) : Error(Message);