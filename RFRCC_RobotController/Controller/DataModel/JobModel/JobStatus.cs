using System;

namespace RFRCC_RobotController.Controller.DataModel
{
    /// <summary>
    /// Status and progression of a Job
    /// </summary>
    public class JobStatus
    {
        private JobProgress _Progress;
        private JobModel _ParentJob;

        /// <summary>
        /// Progress of Job
        /// </summary>
        public JobProgress Progress => _Progress;
        /// <summary>
        /// Generic event update if status changes to any stage
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobStatusChange;
        /// <summary>
        /// Event raised if Job File is imported successfully
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobFileImported;
        /// <summary>
        /// Event raised if Job File is parsed successfully
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobFileParsed;
        /// <summary>
        /// Event raised if Job Data is successfully populated from parsed data
        /// </summary>
        public event EventHandler<JobStatusEventArgs> DataPopulated;
        /// <summary>
        /// Event raised if Job processessing is started
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobStarted;
        /// <summary>
        /// Event raised if Job is Paused
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobPaused;
        /// <summary>
        /// Event raised if Job is Aborted
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobAborted;
        /// <summary>
        /// Event raised if Job is completed
        /// </summary>
        public event EventHandler<JobStatusEventArgs> JobFinished;

        /// <summary>
        /// raises event JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnJobStatusChange(JobStatusEventArgs args)
        {
            JobStatusChange?.Invoke(this, args);
        }
        /// <summary>
        /// Raises event JobFileImported & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnFileImported(JobStatusEventArgs args)
        {
            JobFileImported?.Invoke(this, args);
            OnJobStatusChange(args);
        }
        /// <summary>
        /// Raises event JobFileParsed & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnJobFileParsed(JobStatusEventArgs args)
        {
            JobFileParsed?.Invoke(this, args);
            OnJobStatusChange(args);
        }
        /// <summary>
        /// Raises event DataPopulated & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnDataPopulated(JobStatusEventArgs args)
        {
            DataPopulated?.Invoke(this, args);
            OnJobStatusChange(args);
        }
        /// <summary>
        /// Raises event JobStarted & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnJobStarted(JobStatusEventArgs args)
        {
            JobStarted?.Invoke(this, args);
            OnJobStatusChange(args);
        }
        /// <summary>
        /// Raises event JobPaused & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnJobPaused(JobStatusEventArgs args)
        {
            JobPaused?.Invoke(this, args);
            OnJobStatusChange(args);
        }
        /// <summary>
        /// Raises event JobAborted & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnJobAborted(JobStatusEventArgs args)
        {
            JobAborted?.Invoke(this, args);
            OnJobStatusChange(args);
        }
        /// <summary>
        /// Raises event JobFinished & JobStatusChange
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnJobFinished(JobStatusEventArgs args)
        {
            JobFinished?.Invoke(this, args);
            OnJobStatusChange(args);
        }


        /// <summary>
        /// construct jobstatus object with connection to a jobmodel
        /// </summary>
        /// <param name="Job"></param>
        public JobStatus(JobModel Job)
        {
            _ParentJob = Job;
        }

        /// <summary>
        /// Call when Job has successfully imported job file information
        /// </summary>
        internal void FileImported()
        {
            if (_Progress < JobProgress.WaitingToParse)
            {
                _Progress = JobProgress.WaitingToParse;
                OnFileImported(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// call when file has been parsed by importer
        /// </summary>
        internal void FileParsed()
        {
            if (_Progress < JobProgress.WaitingToPopulateJobData)
            {
                _Progress = JobProgress.WaitingToPopulateJobData;
                OnJobFileParsed(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// call when job data has been populated
        /// </summary>
        internal void JobDataPopulated()
        {
            if (_Progress < JobProgress.WaitingForRobotConnection)
            {
                _Progress = JobProgress.WaitingForRobotConnection;
                if (_ParentJob._parentController.ControllerConnected) _Progress = JobProgress.WaitingForJobStart;

                OnDataPopulated(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// call when job has been successfully started
        /// </summary>
        internal void Started()
        {
            if (_Progress < JobProgress.JobProcessing)
            {
                _Progress = JobProgress.JobProcessing;
                OnJobStarted(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// call when job has been paused
        /// </summary>
        internal void Paused()
        {
            if (_Progress == JobProgress.JobProcessing)
            {
                _Progress = JobProgress.JobPaused;
                OnJobPaused(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// Call when job is being cancelled
        /// </summary>
        internal void Aborting()
        {
            _Progress = JobProgress.JobAborting;
            OnJobAborted(new JobStatusEventArgs(_ParentJob));
        }
        /// <summary>
        /// Call when job is being cancelled
        /// </summary>
        internal void Cancelled()
        {
            if (_Progress == JobProgress.JobAborting)
            {
                _Progress = JobProgress.JobCancelled;
                OnJobFinished(new JobStatusEventArgs(_ParentJob));
            }
            else
            {
                _Progress = JobProgress.JobAborting;
                OnJobAborted(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// Call when job is ending due to completion or cancellation
        /// </summary>
        /// <param name="Cancelled">If job is being closed due to cancellation</param>
        internal void JobEnd(bool Cancelled = false)
        {
            if (Cancelled)
            {
                if (_Progress == JobProgress.JobAborting)
                {
                    _Progress = JobProgress.JobCancelled;
                    OnJobFinished(new JobStatusEventArgs(_ParentJob));
                }
                else
                {
                    _Progress = JobProgress.JobAborting;
                    OnJobAborted(new JobStatusEventArgs(_ParentJob));
                }
            }
            else
            {
                _Progress = JobProgress.JobFinished;
                OnJobFinished(new JobStatusEventArgs(_ParentJob));
            }
        }
        /// <summary>
        /// call when robot is connected to robot controller library
        /// </summary>
        internal void RobotConnected()
        {
            if (_Progress == JobProgress.WaitingForRobotConnection)
            {
                _Progress = JobProgress.WaitingForJobStart;
            }
        }

    }
}
