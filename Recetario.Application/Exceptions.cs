using System;

namespace Recetario.Application;

public class NotFoundException : Exception 
{ 
    public NotFoundException(string message) : base(message) { } 
}

public class ConflictException : Exception 
{ 
    public ConflictException(string message) : base(message) { } 
}



public class BadRequestException : Exception { public BadRequestException(string m) : base(m) { } }
public class PayloadTooLargeException : Exception { public PayloadTooLargeException(string m) : base(m) { } }
public class UnsupportedMediaTypeException : Exception { public UnsupportedMediaTypeException(string m) : base(m) { } }
