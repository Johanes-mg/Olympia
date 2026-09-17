# 🏛️ Olympia - Quiz Mythologie Grecque

Jeu de quiz Windows sur la **mythologie grecque**, développé en **C#** avec **WPF** et **.NET 8.0**.

Testez vos connaissances à travers **360 questions** réparties en 3 difficultés, 4 chapitres et 2 niveaux par chapitre.

---

## 🎮 Fonctionnalités

- 🏛️ **360 questions** sur la mythologie grecque
- 🎯 **3 difficultés** : Mortel, Demi-Dieu, Divin
- 📚 **4 chapitres** par difficulté (2 niveaux de 15 questions chacun)
- ❤️ **9 vies** renouvelées à chaque chapitre
- 🎵 **Musique de fond** avec bouton mute
- ✨ **Animations fluides**
- 🎨 **Style grec antique** (police Cinzel, or/marbre)
- 📦 **Exe autonome**

---

## 📸 Captures d'écran

| Écran                                   | Description         |
| --------------------------------------- | ------------------- |
| <img src="captures/1.png" width="400"/> | Menu principal      |
| <img src="captures/2.png" width="400"/> | Menu principal      |
| <img src="captures/3.png" width="400"/> | Choix de difficulté |
| <img src="captures/4.png" width="400"/> | Choix de chapitre   |
| <img src="captures/5.png" width="400"/> | Question de quiz    |
| <img src="captures/6.png" width="400"/> | Mauvaise réponse    |
| <img src="captures/7.png" width="400"/> | Bonne réponse       |
| <img src="captures/8.png" width="400"/> | Écran de fin        |

---

## 🛠️ Prérequis

| Outil                | Version                                                               |
| -------------------- | --------------------------------------------------------------------- |
| Windows              | 10/11 (64 bits)                                                       |
| .NET Desktop Runtime | 8.0 ([télécharger](https://dotnet.microsoft.com/download/dotnet/8.0)) |

---

## 📥 Installation

1. Téléchargez `Olympia.exe` depuis la section **Releases**
2. Installez le **.NET 8 Desktop Runtime** si pas installé
3. Double-cliquez sur `Olympia.exe`

---

## 🎯 Comment jouer

1. **Menu** → Cliquez sur **CLIQUER POUR COMMENCER**
2. **Difficulté** → Mortel / Demi-Dieu / Divin
3. **Chapitre** → 1, 2, 3 ou 4
4. **Niveau 1** → 15 questions, puis **Niveau 2** → 15 questions
5. **9 vies** au début du chapitre, non renouvelées entre les niveaux
6. **Victoire** : terminer le chapitre avec au moins 1 vie

---

## 🔧 Compilation

```bash
git clone https://github.com/Johanes-mg/Olympia.git
cd Olympia
dotnet restore
dotnet publish -c Release
```

## 👤 Auteur

[Johanes-mg](https://github.com/Johanes-mg/)
