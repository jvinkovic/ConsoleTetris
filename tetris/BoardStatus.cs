namespace tetris
{
    /// <summary>
    /// indicates status of the board
    /// </summary>
    public enum BoardStatus
    {
        /// <summary>
        /// board succesfull updated
        /// </summary>
        Success,

        /// <summary>
        /// board could not update
        /// </summary>
        Fail,

        /// <summary>
        /// board successfully updated and shape hit the bottom
        /// </summary>
        HitBottom
    }
}