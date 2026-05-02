using System;
using Newtonsoft.Json;
using UnityEngine;
using static Newtonsoft.Json.MemberSerialization;

namespace PersistentLog {
    [JsonObject(OptIn)]
    internal class LogItemInfo {
        [JsonProperty("timestamp")] DateTime _timestamp;
        [JsonProperty("type")] LogType _type;
        [JsonProperty("message")] string _message;
        [JsonProperty("stackTrace")] string _stackTrace;

        public DateTime Timestamp => _timestamp;
        public LogType Type => _type;
        public string Message => _message;
        public string StackTrace => _stackTrace;

        public LogItemInfo() {
            _message = "";
            _stackTrace = "";
        }

        [JsonConstructor]
        public LogItemInfo(DateTime timestamp, LogType type, string message, string stackTrace)
            => (_timestamp, _type, _message, _stackTrace) = (timestamp, type, message, stackTrace);

        public void Reset() {
            _message = "";
            _stackTrace = "";
        }
    }
}