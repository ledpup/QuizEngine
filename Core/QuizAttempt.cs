using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace QuizEngine
{
    public class QuizAttempt
    {
        public QuizAttempt(List<QuizQuestion> quizQuestions, bool practiceMode)
        {
            QuizQuestions = quizQuestions;
            PracticeMode = practiceMode;
            DispatcherTimerSetup();
        }

        public Timer DispatcherTimer;
        int _timesToTick;
        private int _timesTicked;
        private DateTimeOffset _quizEnds;
        public TimeSpan QuizTimeRemaining;

        void DispatcherTimerSetup()
        {
            DispatcherTimer = new Timer();// { Interval = new TimeSpan(0, 0, 1) };

            _timesToTick = 60 * QuizQuestions.Count;
            QuizStart = DateTimeOffset.Now;
            DispatcherTimer.Start();
            _quizEnds = QuizStart.AddSeconds(_timesToTick);
        }

        public void EndQuiz()
        {
            DispatcherTimer.Stop();

            if (QuizEnd != DateTimeOffset.MinValue)
                throw new Exception("The quiz has already ended.");

            QuizEnd = DateTimeOffset.Now;
        }

        public bool PracticeMode;

        public List<QuizQuestion> QuizQuestions;
        public DateTimeOffset QuizStart;
        public DateTimeOffset QuizEnd;

        private float Score
        {
            get { return QuizQuestions.Where(x => x.SelectedAnswer != null).Sum(x => x.SelectedAnswer.Score); }
        }

        private float ScoreOutOf { get { return QuizQuestions.Sum(x => x.Answers.Single(a => a.Score > 0).Score); } }
        private double ScorePercentage { get { return Math.Round((Score / ScoreOutOf) * 100); } }

        public string QuizResult()
        {
            return string.Format("{0}/{1} ({2}%)", Score, ScoreOutOf, ScorePercentage);
        }

        public int MaxQuizDuration
        {
            get { return QuizQuestions.Count; }
        }
        public bool QuizDurationExpired
        {
            get { return QuizDuration.Minutes > MaxQuizDuration; }
        }

        public TimeSpan QuizDuration
        {
            get
            {
                var duration = QuizEnd.Subtract(QuizStart);
                return new TimeSpan(0, duration.Hours, duration.Minutes, duration.Seconds);
            }
        }

        internal bool UpdateTimeRemainingOnQuiz()
        {
            _timesTicked++;

            var timeTaken = QuizStart.AddSeconds(_timesTicked);
            QuizTimeRemaining = _quizEnds - timeTaken;

            return _timesTicked >= _timesToTick;
        }
    }
}
