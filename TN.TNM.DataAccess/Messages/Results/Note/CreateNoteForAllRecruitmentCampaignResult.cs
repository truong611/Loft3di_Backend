﻿using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Note
{
    public class CreateNoteForAllRecruitmentCampaignResult : BaseResult
    {
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
