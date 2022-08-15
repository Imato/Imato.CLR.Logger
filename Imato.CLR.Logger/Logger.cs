using Microsoft.SqlServer.Server;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace Imato.CLR.Logger
{
    public class Logger : IDisposable
    {
        private readonly ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        private readonly LogLevel _level = LogLevel.Error;

        public Logger(LogLevel level = LogLevel.Error)
        {
            _level = level;
        }

        public void Debug(string message, SqlProcedure procedure = null)
        {
            Log(message, LogLevel.Debug, procedure);
        }

        public void Info(string message, SqlProcedure procedure = null)
        {
            Log(message, LogLevel.Info, procedure);
        }

        public void Warning(string message, SqlProcedure procedure = null)
        {
            Log(message, LogLevel.Warning, procedure);
        }

        public void Error(string message, SqlProcedure procedure = null)
        {
            Log(message, LogLevel.Error, procedure);
        }

        public void Error(Exception ex, SqlProcedure procedure = null)
        {
            Log(ex.ToString(), LogLevel.Error, procedure);
        }

        private void Log(string message, LogLevel level = LogLevel.Info, SqlProcedure procedure = null)
        {
            if (_level <= level)
            {
                var command = "";
                if (procedure != null)
                    command = $" SQL command {procedure.Id}";

                var m = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} {level} {command}: {message}";

                _messages.Enqueue(m);
            }
        }

        private static SqlDataRecord GetDataRecord(SqlProcedure procedure)
        {
            var record = new SqlDataRecord(new SqlMetaData("Id", SqlDbType.Int),
                new SqlMetaData("Text", SqlDbType.NVarChar, 4000),
                new SqlMetaData("IsSuccess", SqlDbType.Bit),
                new SqlMetaData("Error", SqlDbType.NVarChar, 4000));

            record.SetInt32(0, procedure.Id);
            record.SetSqlString(1, procedure.Text.GetSqlString());
            record.SetBoolean(2, procedure.IsSuccess);
            record.SetSqlString(3, procedure.Error.GetSqlString());

            return record;
        }

        /// <summary>
        /// Return row with procedure information
        /// </summary>
        /// <param name="procedure"></param>
        public void Output(SqlProcedure procedure)
        {
            SqlContext.Pipe.Send(GetDataRecord(procedure));
        }

        /// <summary>
        /// Return row with data
        /// </summary>
        /// <param name="recordFactory">Function create new row - SqlDataRecord</param>
        /// <param name="fillFactory">Action to fill SqlDataRecord, row information</param>
        public void Output(Func<SqlDataRecord> recordFactory, Action<SqlDataRecord> fillFactory)
        {
            var record = recordFactory();
            fillFactory(record);
            SqlContext.Pipe.Send(record);
        }

        /// <summary>
        /// Push all log messages to output
        /// </summary>
        public void PrintMessages()
        {
            while (_messages.TryDequeue(out var message) && !string.IsNullOrEmpty(message))
            {
                SqlContext.Pipe?.Send(message.GetSqlString());
            }
        }

        public void Dispose()
        {
            PrintMessages();
        }
    }
}