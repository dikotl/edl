namespace Edl.Core.Interpreter.Intrinsics;

public abstract class FileOperationCommand : IIntrinsic
{
    // internal required string Filename { get; set; }

    // protected FileProcedure(LineInfo callSite, List<Value> args, int argsCount = 1) : base(callSite, args, argsCount)
    // {
    //     if (args[0] is not Value.String filename)
    //     {
    //         throw new TypeMismatchException("expected 'String' for filename", CallSite);
    //     }
    //     Filename = filename.Value;
    // }

    public void Call(Context context, Value[] args)
    {
        // try
        // {
        //     return ExecuteFileOperation();
        // }
        // catch (FileNotFoundException)
        // {
        //     throw new PanicException(PanicTags.FileNotFoundError, $"file not found: {Filename}", CallSite);
        // }
        // catch (UnauthorizedAccessException)
        // {
        //     throw new PanicException(PanicTags.PermissionDeniedError, $"unauthorized file access: {Filename}", CallSite);
        // }
        // catch (IOException)
        // {
        //     throw new PanicException(PanicTags.FileNotExistError, $"I\\O error occurred: {Filename}", CallSite);
        // }
        // catch
        // {
        //     throw new PanicException(PanicTags.IoError, $"unknown error occurred while executing I\\O: {Filename}", CallSite);
        // }
    }

    protected abstract void ExecuteFileOperation();
}
