﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EOE_WebAPI.Models
{
    public class User
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public string DisplayName { get; set; }
        public DateTime? Birth { get; set; }
        public bool? Sex { get; set; } // 0: female, 1: male

        public virtual Account Account { get; set; } // Để truy cập tài khoản
        public virtual ICollection<GameScore> GameScores { get; set; } // Để truy cập danh sách điểm số của người dùng
    }

}
