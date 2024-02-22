namespace GameLogic
{
    public class GameScores
    {
        private int m_PlayerOneScores;
        private int m_PlayerTwoScores;

        public GameScores()
        {
            m_PlayerOneScores = 0;
            m_PlayerTwoScores = 0;
        }

        public int PlayerOneScores
        {
            get { return m_PlayerOneScores; }
            set { m_PlayerOneScores = value; }
        }

        public int PlayerTwoScores
        {
            get { return m_PlayerTwoScores; }
            set { m_PlayerTwoScores = value; }
        }

        public bool IsZeroTie()
        {
            return m_PlayerOneScores == 0 && m_PlayerTwoScores == 0;
        }

        public void AddOnePointToOpponent(eShapes i_CurrentShape)
        {
            if (i_CurrentShape == eShapes.X)
            {
                m_PlayerTwoScores++;
            }
            else
            {
                m_PlayerOneScores++;
            }
        }
    }
}
