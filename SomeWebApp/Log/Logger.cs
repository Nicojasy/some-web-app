using System;
using Microsoft.Extensions.Logging;

namespace SomeWebApp.LogExtension
{
    public static class Logger
    {
        private static readonly Action<ILogger, string, Exception> _info;
        private static readonly Action<ILogger, string, Exception> _infoDatabaseQueryReturnedNull;
        private static readonly Action<ILogger, string, Exception> _infoDatabaseQueryReturnedZeroAffectedRows;
        private static readonly Action<ILogger, string, string, Exception> _infoPasswordsMismatch;
        private static readonly Action<ILogger, string, Exception> _warn;
        private static readonly Action<ILogger, string, Exception> _warnNeedsUpgrade;
        private static readonly Action<ILogger, string, Exception> _warnBadPasswordFormatWithException;
        private static readonly Action<ILogger, string, Exception> _warnDatabaseQueryReturnedZeroAffectedRows;

        static Logger()
        {
            _info = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(200, nameof(Info)),
                "{message}");

            _infoDatabaseQueryReturnedNull = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(201, nameof(InfoDatabaseQueryReturnedNull)),
                "The database query returned null. {message}");

            _infoDatabaseQueryReturnedZeroAffectedRows = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(202, nameof(InfoDatabaseQueryReturnedZeroAffectedRows)),
                "The database query returned 0 affected rows. {message}");

            _infoPasswordsMismatch = LoggerMessage.Define<string, string>(
                LogLevel.Information,
                new EventId(203, nameof(InfoPasswordsMismatch)),
                "The hash does not match the password => hash: '{hash}', password: '{password}'");

            _warn = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(300, nameof(Info)),
                "{message}");

            _warnNeedsUpgrade = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(301, nameof(WarnNeedsUpgrade)),
                "{message}");

            _warnBadPasswordFormatWithException = LoggerMessage.Define<string>(
                LogLevel.Information,
                new EventId(302, nameof(WarnBadPasswordFormatWithException)),
                "ex => hash: '{hash}'");

            _warnDatabaseQueryReturnedZeroAffectedRows = LoggerMessage.Define<string>(
                LogLevel.Warning,
                new EventId(303, nameof(WarnDatabaseQueryReturnedZeroAffectedRows)),
                "{message}");
        }

        /*
        internal static class Events
        {
            public static readonly EventId ev = new EventId(1, nameof(Info));
        }
        */

        public static void Info(this ILogger logger, string message)
        {
            _info(logger, message, null);
        }

        public static void InfoDatabaseQueryReturnedNull(this ILogger logger, string message)
        {
            _infoDatabaseQueryReturnedNull(logger, message, null);
        }

        public static void InfoDatabaseQueryReturnedZeroAffectedRows(this ILogger logger, string message)
        {
            _infoDatabaseQueryReturnedZeroAffectedRows(logger, message, null);
        }

        public static void InfoPasswordsMismatch(this ILogger logger, string hash, string password)
        {
            _infoPasswordsMismatch(logger, hash, password, null);
        }

        public static void Warn(this ILogger logger, string message)
        {
            _warnNeedsUpgrade(logger, message, null);
        }

        public static void WarnNeedsUpgrade(this ILogger logger, string message)
        {
            _warnNeedsUpgrade(logger, message, null);
        }

        public static void WarnBadPasswordFormatWithException(this ILogger logger, string hash, FormatException ex)
        {
            _warnBadPasswordFormatWithException(logger, hash, ex);
        }

        public static void WarnDatabaseQueryReturnedZeroAffectedRows(this ILogger logger, string message)
        {
            _warnDatabaseQueryReturnedZeroAffectedRows(logger, message, null);
        }

    }
}