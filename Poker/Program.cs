﻿using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace Poker
{
	class Program
	{
		// -----------------------
		// DECLARATION DES DONNEES
		// -----------------------
		// Importation des DLL (librairies de code) permettant de gérer les couleurs en mode console
		[DllImport("kernel32.dll")]
		public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
		[DllImport("kernel32.dll")]
		public static extern IntPtr GetStdHandle(uint nStdHandle);
		static uint STD_OUTPUT_HANDLE = 0xfffffff5;
		static IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);

		// Pour utiliser la fonction C 'getchar()' : saisie d'un caractère
		[DllImport("msvcrt.dll")]
		static extern int _getche();

		//-------------------
		// TYPES DE DONNEES
		//-------------------
 
		// Fin du jeu
		public static bool fin = false;

		// Codes COULEUR
		public enum couleur
		{
			VERT = 10,
			ROUGE = 12,
			JAUNE = 14,
			BLANC = 15,
			NOIRE = 0,
			ROUGESURBLANC = 252,
			NOIRESURBLANC = 240}
		;

		// Coordonnées pour l'affichage
		public struct coordonnees
		{
			public int x;
			public int y;
		}

		// Une carte
		public struct carte
		{
			public char valeur;
			public int famille;
		}

		// Liste des combinaisons possibles
		public enum combinaison
		{
			RIEN,
			PAIRE,
			DOUBLE_PAIRE,
			BRELAN,
			QUINTE,
			FULL,
			COULEUR,
			CARRE,
			QUINTE_FLUSH}

		;

		// Valeurs des cartes : As, Roi,...
		public static char[] valeurs = {
			'A', 'R', 'D', 'V', 'X', '9', '8', '7', '6', '5', '4', '3', '2'
		};

		// Codes ASCII (3 : cœur, 4 : carreau, 5 : trèfle, 6 : pique)
		public static int[] familles = { 3, 4, 5, 6 };

		// Numéros des cartes à échanger
		public static int[] echange = { 0, 0, 0, 0 };

		// Jeu de 5 cartes
		public static carte[] MonJeu = new carte[5];
        	

		//----------
		// FONCTIONS
		//----------

		// Génère aléatoirement une carte : {valeur;famille}
		// Retourne une expression de type "structure carte"
		public static carte Tirage()
		{
			Random random = new Random();
			carte nouvelleCarte = new carte {
				valeur = valeurs[random.Next(0, valeurs.Length)],
				famille = familles[random.Next(0, familles.Length)]
			};
			return nouvelleCarte;
		}

		// Indique si une carte est déjà présente dans le jeu
		// Paramètres : une carte, le jeu 5 cartes, le numéro de la carte dans le jeu
		// Retourne un entier (booléen)
		public static bool CarteUnique(carte uneCarte, carte[] unJeu, int numero)
		{
			for (int i = 0; i < numero; i++) {
				if (uneCarte.valeur == unJeu[i].valeur && uneCarte.famille == unJeu[i].famille) {
					return false;
				}
			}
			return true;
		}

		// Calcule et retourne la COMBINAISON (paire, double-paire... , quinte-flush)
		// pour un jeu complet de 5 cartes.
		// La valeur retournée est un élément de l'énumération 'combinaison' (=constante)
		public static combinaison ChercheCombinaison(carte[] unJeu)
		{
			Array.Sort(unJeu, (c1, c2) => c1.valeur.CompareTo(c2.valeur));

			if (HasPaire(unJeu)) {
				return combinaison.PAIRE;
			}
			if (HasDoublePaire(unJeu)) {
				return combinaison.DOUBLE_PAIRE;
			}
			if (HasBrelan(unJeu)) {
				return combinaison.BRELAN;
			}
			return combinaison.RIEN;
		}
        
		private static bool HasPaire(carte[] unJeu)
		{
			for (int i = 0; i < unJeu.Length - 1; i++) {
				if (unJeu[i].valeur == unJeu[i + 1].valeur) {
					return true;
				}
			}
			return false;
		}

		private static bool HasDoublePaire(carte[] unJeu)
		{
			int countPairs = 0;

			for (int i = 0; i < unJeu.Length - 1; i++) {
				if (unJeu[i].valeur == unJeu[i + 1].valeur) {
					countPairs++;
					i++;
				}
			}

			return countPairs == 2;
		}
        
		private static bool HasBrelan(carte[] unJeu)
		{
			for (int i = 0; i < unJeu.Length - 2; i++) {
				if (unJeu[i].valeur == unJeu[i + 1].valeur && unJeu[i].valeur == unJeu[i + 2].valeur) {
					return true;
				}
			}
			return false;
		}

		// Echange des cartes
		// Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger
		private static void EchangeCartes(carte[] unJeu, int[] numeros)
{
    if (numeros.Length != 4)
    {
        Console.WriteLine("Erreur : Vous devez fournir un tableau de 4 numéros de carte à échanger.");
        return;
    }

    for (int i = 0; i < 4; i++)
    {
        int numeroCarteAEchanger = numeros[i];

        if (numeroCarteAEchanger >= 1 && numeroCarteAEchanger <= 4)
        {
            // Générer une nouvelle carte pour remplacer la carte à échanger
            carte nouvelleCarte = Tirage();

            // S'assurer que la nouvelle carte est unique dans le jeu
            while (!CarteUnique(nouvelleCarte, unJeu, numeroCarteAEchanger - 1))
            {
                nouvelleCarte = Tirage();
            }

            // Remplacer la carte à échanger par la nouvelle carte
            unJeu[numeroCarteAEchanger - 1] = nouvelleCarte;
        }
        else
        {
            Console.WriteLine("Erreur : Numéro de carte invalide pour l'échange {i + 1}. Veuillez réessayer.");
        }
    }
}




		// Pour afficher le Menu principal
		private static void AfficheMenu()
		{

			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("+--------------+");
			Console.WriteLine("|              |");
			Console.WriteLine("|    POKER     |");
			Console.WriteLine("|              |");
			Console.WriteLine("|  1. Jouer    |");   
			Console.WriteLine("|  2. Scores   |");
			Console.WriteLine("|  3. Quitter  |");
			Console.WriteLine("|              |");
			Console.WriteLine("+--------------+");
            
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Votre choix : ");
			Console.ResetColor();
		}

		// Jouer au Poker
		// Ici que vous appelez toutes les fonctions permettant de jouer au poker
		private static void JouerAuPoker()
		{
		    Console.Clear();
		    // Initialiser le jeu de cartes
		    TirageDuJeu(MonJeu);
		    // Afficher le jeu initial
		    AffichageCarte();
		    // Déclarer et initialiser le tableau 'numeros'
		    int[] numeros = { 1, 2, 3, 4 };
		    for (int i = 0; i < 4; i++)
		    {
		        // Demander les cartes à échanger
		        Console.WriteLine("Voulez-vous échanger des cartes ? (Oui/Non)");
		        string choix = Console.ReadLine();
		
		        if (choix.ToLower() == "oui")
		        {
		            Console.WriteLine("Entrez les numéros des cartes à échanger (1-4) pour l'échange ");
		            for (int j = 0; j < 4; j++)
		            {
		                numeros[j] = int.Parse(Console.ReadLine());
		            }
					Console.Clear();
		            EchangeCartes(MonJeu, numeros);
		            AffichageCarte();
		        }
		        else
		        {
		            break;
		        }
		    }
		    AfficheResultat(MonJeu);
		    EnregistrerJeu(MonJeu);
		}



		// Tirage d'un jeu de 5 cartes
		// Paramètre : le tableau de 5 cartes à remplir
		private static void TirageDuJeu(carte[] unJeu)
		{
			Random rdn = new Random();

			for (int i = 0; i < unJeu.Length; i++) {
				// Tirage de la carte n°i (le jeu doit être sans doublons !)
				carte nouvelleCarte = Tirage();

				// S'assurer que la carte est unique dans le jeu
				while (!CarteUnique(nouvelleCarte, unJeu, i)) {
					nouvelleCarte = Tirage();
				}

				// Ajouter la carte au jeu
				unJeu[i] = nouvelleCarte;
			}
		}

		// Affiche à l'écran une carte {valeur;famille}
		private static void AffichageCarte()
		{
			//----------------------------
			// TIRAGE D'UN JEU DE 5 CARTES
			//----------------------------
			int left = 0;
			int c = 1;
			// Tirage aléatoire de 5 cartes
			for (int i = 0; i < 5; i++) {
				// Tirage de la carte n°i (le jeu doit être sans doublons !)

				// Affichage de la carte
				if (MonJeu[i].famille == 3 || MonJeu[i].famille == 4)
					SetConsoleTextAttribute(hConsole, 252);
				else
					SetConsoleTextAttribute(hConsole, 240);
				Console.SetCursorPosition(left, 5);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '+', '-', '-', '-', '-', '-', '-', '-', '-', '-', '+');
				Console.SetCursorPosition(left, 6);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
				Console.SetCursorPosition(left, 7);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
				Console.SetCursorPosition(left, 8);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
				Console.SetCursorPosition(left, 9);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
				Console.SetCursorPosition(left, 10);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', (char)MonJeu[i].famille, '|');
				Console.SetCursorPosition(left, 11);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
				Console.SetCursorPosition(left, 12);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
				Console.SetCursorPosition(left, 13);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
				Console.SetCursorPosition(left, 14);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
				Console.SetCursorPosition(left, 15);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '+', '-', '-', '-', '-', '-', '-', '-', '-', '-', '+');
				Console.SetCursorPosition(left, 16);
				SetConsoleTextAttribute(hConsole, 10);
				Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", ' ', ' ', ' ', ' ', ' ', c, ' ', ' ', ' ', ' ', ' ');
				left = left + 15;
				c++;
			}
		}
		// Enregistre le score dans le fichier texte
		private static void EnregistrerJeu(carte[] unJeu)
		{

			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Enregistrer le jeu ? <o/n>");
			string E = Console.ReadLine();
			if (E.ToLower() == "o") {
				using (BinaryWriter writer = new BinaryWriter(new FileStream("Scores.txt", FileMode.Append, FileAccess.Write))) {
					StringBuilder sb = new StringBuilder();
					Console.ForegroundColor = ConsoleColor.Green;
					Console.WriteLine("Saisir votre PSEUDO");
					string pseudo = Console.ReadLine();
					
					foreach (carte c in unJeu) {
						sb.Append(c.valeur).Append(c.famille);
					}
					long score = (long)sb.ToString().GetHashCode() * 5;
					writer.Write(pseudo);
					writer.Write(score);
					switch (ChercheCombinaison(unJeu)) {
						case combinaison.RIEN:
							writer.Write("Pseudo :" + pseudo + " il n'a rien du tout... désolé!" + ", Score : " + score);
							break;
						case combinaison.PAIRE:
							writer.Write("Pseudo :" + pseudo + " il n'a une simple paire..." + ", Score : " + score);
							break;
						case combinaison.DOUBLE_PAIRE:
							writer.Write("Pseudo :" + pseudo + " il n'a une double paire; on peut espérer... " + ", Score : " + score);
							break;
						case combinaison.BRELAN:
							writer.Write("Pseudo :" + pseudo + " il n'a un brelan; pas mal..." + ", Score : " + score);
							break;
						case combinaison.QUINTE:
							writer.Write("Pseudo :" + pseudo + " il n'a une quinte; bien!" + ", Score : " + score);
							break;
						case combinaison.FULL:
							writer.Write("Pseudo :" + pseudo + " il n'a un full; ouahh!" + ", Score : " + score);
							break;
						case combinaison.COULEUR:
							writer.Write("Pseudo :" + pseudo + " il n'a une couleur; bravo!" + ", Score : " + score);
							break;
						case combinaison.CARRE:
							writer.Write("Pseudo :" + pseudo + " il n'a un carré; champion! " + ", Score : " + score);
							break;
						case combinaison.QUINTE_FLUSH:
							writer.Write("Pseudo :" + pseudo + " il n'a une quinte-flush; royal! " + ", Score : " + score);
							break;
					}
					writer.Close();
				}
				
				
				
			} else {
				Console.Clear();
			}		
		}
		// Affiche le Scores
		private static void VoirScores()
		{
			using (BinaryReader reader = new BinaryReader(new FileStream("Scores.txt", FileMode.Open, FileAccess.Read))) {

				try {
					Console.ResetColor();
					Console.WriteLine("");
					Console.WriteLine("+-----------+");
					Console.WriteLine("|  SCORES   |");
					Console.WriteLine("+-----------+");

					while (true) {

						if (reader.BaseStream.Position + sizeof(int) > reader.BaseStream.Length)
							break;
						string pseudo = reader.ReadString();
						if (reader.BaseStream.Position + sizeof(long) > reader.BaseStream.Length)
							break;
						long score = reader.ReadInt64();
						Console.WriteLine("Pseudo: " + pseudo + ", Score: " + score);
					}
				} catch (EndOfStreamException) {
				}
			}
		}
		
		
		// Affiche résultat
		private static void AfficheResultat(carte[] unJeu)
		{
			SetConsoleTextAttribute(hConsole, 012);
			Console.Clear();
			Console.Write("RESULTAT - Vous avez : ");
			try {
				switch (ChercheCombinaison(unJeu)) {
					case combinaison.RIEN:
						Console.WriteLine("rien du tout... désolé!");
						break;
					case combinaison.PAIRE:
						Console.WriteLine("une simple paire...");
						break;
					case combinaison.DOUBLE_PAIRE:
						Console.WriteLine("une double paire; on peut espérer...");
						break;
					case combinaison.BRELAN:
						Console.WriteLine("un brelan; pas mal...");
						break;
					case combinaison.QUINTE:
						Console.WriteLine("une quinte; bien!");
						break;
					case combinaison.FULL:
						Console.WriteLine("un full; ouahh!");
						break;
					case combinaison.COULEUR:
						Console.WriteLine("une couleur; bravo!");
						break;
					case combinaison.CARRE:
						Console.WriteLine("un carré; champion!");
						break;
					case combinaison.QUINTE_FLUSH:
						Console.WriteLine("une quinte-flush; royal!");
						break;
				}
				;
			} catch {
			}	
			//Demande_Enregistrer(unJeu);
		}
		//--------------------
		// Fonction PRINCIPALE
		//--------------------
		static void Main(string[] args)
		{
			//---------------
			// BOUCLE DU JEU
			//---------------
			char reponse;
			while (true) {
				AfficheMenu();
				reponse = (char)_getche();
				if (reponse != '1' && reponse != '2' && reponse != '3') {
					Console.Clear();
					AfficheMenu();
				} else {
					SetConsoleTextAttribute(hConsole, 015);
					// Jouer au Poker
					if (reponse == '1') {
						int i = 0;
						JouerAuPoker();
					}
					if (reponse == '2')
						VoirScores();

					if (reponse == '3')
						break;
				}
			}
			Console.Clear();
		}
	}
}