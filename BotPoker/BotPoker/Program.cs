using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BotPoker
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            
        }
        static void mainBis()
        {
            //lancer le canevas du jeu

            //lancer une nouvelle partie
            // A la main pour le moment

            ///Début boucle
                //attendre notre tour 

                //jouer
                    //Recupérer la situation
                    //lancer calculon
                    //effectuer l'action determinée

                //Attendre la la fin du jeu
            ///Fin boucle
        }
    }
}
