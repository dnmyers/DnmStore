using System;
using System.Runtime.Serialization;

namespace DnmStore.Server.Exceptions;

[Serializable]
internal class UserNotFoundException : Exception {
    public UserNotFoundException() {
    }

    public UserNotFoundException(string? message) : base(message) {
    }

    public UserNotFoundException(string? message, Exception? innerException) : base(message, innerException) {
    }

#pragma warning disable SYSLIB0051 // Type or member is obsolete
    protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) {
    }
#pragma warning restore SYSLIB0051 // Type or member is obsolete
}