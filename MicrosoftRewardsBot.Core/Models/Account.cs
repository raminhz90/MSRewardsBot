using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace MSRewardsBOT.Core.Models
{
    public class Account : ObservableValidator
    {
        private string userName;
        private string password;
        private int _PCPoints;
        private int mobilePoints;
        private int totalPoints;
        private DateTime lastRun;
        private bool isActive;
        private bool isError;
        private int _ID;
        private int progress;
        private List<string> cmdLog;


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get => _ID; set => SetProperty(ref _ID, value, true); }
        [Column("EmailAddress")][Required]
        public string UserName { get => userName; set => SetProperty(ref userName, value, true); }
        [Required]
        public string Password { get => password; set => SetProperty(ref password, value, true); }
        public int PCPoints { get => _PCPoints; set => SetProperty(ref _PCPoints, value, true); }
        public int MobilePoints { get => mobilePoints; set => SetProperty(ref mobilePoints, value, true); }
        public int TotalPoints { get => totalPoints; set => SetProperty(ref totalPoints, value, true); }
        public DateTime LastRun { get => lastRun; set => SetProperty(ref lastRun, value, true); }
        [Required]
        public bool IsActive { get => isActive; set => SetProperty(ref isActive, value, true); }
        [Required]
        public bool IsError { get => isError; set => SetProperty(ref isError, value, true); }
        [NotMapped]
        public int Progress { get => progress; set => SetProperty(ref progress, value, true); }
        [NotMapped]
        public List<string> CmdLog { get => cmdLog; set => SetProperty(ref cmdLog, value, true); }


    }
}
