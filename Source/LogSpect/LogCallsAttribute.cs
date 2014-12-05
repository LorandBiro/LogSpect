namespace LogSpect
{
    using System;
    using LogSpect.Logging;

    /// <summary>
    /// Marks the method for logging instrumentation.
    /// </summary>
    /// <remarks>
    /// To enable the logging you have to run the LogSpectRewriter tool on the assembly. (If you installed the LogSpect NuGet package the tool is already
    /// included in the build process.) The tool scans the selected assembly and adds logging instructions for the marked methods. The following method:
    /// <code>
    /// [LogCalls]
    /// public int Foo(int x)
    /// {
    ///     return x * x;
    /// }
    /// </code>
    /// will be transformed like this:
    /// <code>
    /// private static readonly IMethodLogger logger;
    /// 
    /// [LogCalls]
    /// public int Foo(int x)
    /// {
    ///     if (logger == null)
    ///     {
    ///         logger = MethodLoggerFactoryLocator.Factory.Create(methodof(Foo));
    ///     }
    /// 
    ///     int ret;
    ///     object[] args = { x };
    ///     logger.LogEnter(args);
    ///     try
    ///     {
    ///         ret = x * x;
    ///         logger.LogLeave(args, ret);
    ///     }
    ///     catch (Exception e)
    ///     {
    ///         logger.LogException(args, e);
    ///         throw;
    ///     }
    /// 
    ///     return ret;
    /// }
    /// </code>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public class LogCallsAttribute : LogCallsAttributeBase
    {
        /// <summary>
        /// The default normal log level that is used if it's not specified.
        /// </summary>
        public const Level DefaultNormalLogLevel = Level.Trace;

        /// <summary>
        /// The default exception log level that is used if it's not specified.
        /// </summary>
        public const Level DefaultExceptionLogLevel = Level.Warning;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCallsAttribute"/> class.
        /// </summary>
        /// <param name="expectedExceptions">The list of exception types that are not considered as errors. An expected exception will be logged with normal log level.</param>
        public LogCallsAttribute(params Type[] expectedExceptions)
            : base(DefaultNormalLogLevel, DefaultExceptionLogLevel, expectedExceptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCallsAttribute"/> class.
        /// </summary>
        /// <param name="normalLogLevel">The log level of the enter, leave and expected exception events.</param>
        /// <param name="expectedExceptions">The list of exception types that are not considered as errors. An expected exception will be logged with normal log level.</param>
        public LogCallsAttribute(Level normalLogLevel, params Type[] expectedExceptions)
            : base(normalLogLevel, DefaultExceptionLogLevel, expectedExceptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogCallsAttribute"/> class.
        /// </summary>
        /// <param name="normalLogLevel">The log level of the enter, leave and expected exception events.</param>
        /// <param name="exceptionLogLevel">The log level of the unexpected exception events.</param>
        /// <param name="expectedExceptions">The list of exception types that are not considered as errors. An expected exception will be logged with normal log level.</param>
        public LogCallsAttribute(Level normalLogLevel, Level exceptionLogLevel, params Type[] expectedExceptions)
            : base(normalLogLevel, exceptionLogLevel, expectedExceptions)
        {
        }
    }
}
