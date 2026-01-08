using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Base
{
    public abstract class RatableEntity : AuditableEntity
    {
        public decimal AverageRating { get; private set; }

        public int ReviewCount { get; private set; }

        public void UpdateRating(int newRating)
        {
            if (newRating < 1 || newRating > 5)
                throw new ArgumentOutOfRangeException(nameof(newRating), "the Rating shoould be  between 1 or 5  stars");

            decimal totalRatingScore = (AverageRating * ReviewCount) + newRating;
            // benfite of it  Increase the number of participants 
            ReviewCount++;

            // 
            AverageRating = Math.Round(totalRatingScore / ReviewCount , 2);
        }

    }
}
