﻿using System.Collections.Generic;
using TN.TNM.DataAccess.Models.Note;

namespace TN.TNM.DataAccess.Messages.Results.Note
{
    public class CreateNoteForProductionOrderDetailResult : BaseResult
    {
        public List<NoteEntityModel> ListNote { get; set; }
    }
}
