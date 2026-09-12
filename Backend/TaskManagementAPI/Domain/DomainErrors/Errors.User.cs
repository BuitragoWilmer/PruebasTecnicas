using ErrorOr;

namespace Domain.DomainErrors;

public static partial class Errors
{
    public static class User
    {
        public static Error NameCompanyWithBadFormat =>
            Error.Validation("User.Name", "The name of the user has not valid format.");

        public static Error CouldNotCreate =>
            Error.Validation("User.Create", "Could Not Create User.");

        public static Error AlreadyExists =>
            Error.Validation("User.Name", "User already exists.");
        
         public static Error AlreadyExistsId =>
            Error.Validation("User.Id", "User already exists.");
        

         public static Error Notfound =>
            Error.NotFound("User.NotFound", "The User with the provide Id was not found.");

    }

    public static class TaskItem
    {
        public static Error NotFound =>
            Error.NotFound("TaskItem.NotFound", "The task item with the provided Id was not found.");
    }
}