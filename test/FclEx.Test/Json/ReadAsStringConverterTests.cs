﻿using System;
using System.Collections.Generic;
using System.Text;
using FclEx.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace FclEx.Test.Json
{
    public class ReadAsStringConverterTests
    {
        private class Tester
        {
            [JsonConverter(typeof(ReadAsStringConverter))]
            public string MatchId { get; set; }
            [JsonConverter(typeof(ReadAsStringConverter))]
            public string Grades { get; set; }
        }

        private class GradeItem
        {
            public string Grade { get; set; }
            public string LessonId { get; set; }
        }

        [Fact]
        public void Test()
        {
            var json = "{\"matchId\":\"11\",\"grades\":[{\"grade\":\"1\",\"lessonId\":\"123\"}]}";
            var obj = json.ToJToken().ToObject<Tester>();
            var grades = obj.Grades.ToJToken().ToObject<GradeItem[]>();
            Assert.Equal("11", obj.MatchId);
            Assert.Single(grades);
        }
    }
}
