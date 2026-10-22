# Factory Frenzy - PC

**Projet Unity | CESI Rouen | 2026**

Factory Frenzy est un projet développé sous Unity dans le cadre du module Réalité Augmentée / Réalité Virtuelle au CESI Rouen.

Le projet consiste à créer un jeu de parcours d'obstacles en 3D, dans lequel le joueur doit franchir différentes plateformes et atteindre la ligne d'arrivée.

Il comprend également un éditeur de niveaux en réalité virtuelle permettant de créer des parcours et de les exporter au format JSON pour les utiliser dans le jeu PC.

---

## Sommaire

- [Présentation](#présentation)
- [Technologies utilisées](#technologies-utilisées)
- [Fonctionnalités](#fonctionnalités)
- [Architecture du projet](#architecture-du-projet)
- [Installation](#installation)
- [Organisation du développement](#organisation-du-développement)
- [Projet VR](#projet-vr)
- [Équipe](#équipe)

---

## Présentation

Le projet est divisé en deux parties complémentaires :

| Module | Description |
|---|---|
| **PC** | Jeu de parcours en troisième personne avec plateformes, obstacles et gestion des niveaux |
| **VR** | Éditeur de niveaux immersif permettant de placer et configurer les éléments d'un parcours |

Les deux parties utilisent des fichiers JSON pour échanger les données des niveaux : les parcours créés dans l'éditeur VR sont destinés à être importés dans le jeu PC.

## Technologies utilisées

- **Unity 2022.3.19f1** - Moteur de développement
- **C#** - Programmation des fonctionnalités
- **Unity Starter Assets** - Contrôleur du joueur
- **Cinemachine** - Gestion de la caméra
- **JSON** - Sauvegarde et chargement des niveaux
- **Git / GitHub** - Gestion des versions et collaboration
- **Git LFS** - Gestion des fichiers volumineux

## Fonctionnalités

### Jeu PC

**Joueur et caméra**
- Déplacement du personnage en troisième personne
- Caméra suivant le joueur
- Gestion du contrôleur et des interactions

**Course**
- Zone de départ et d'arrivée
- Chronomètre de course
- Gestion des niveaux 1 à 3
- Menus, pause et réglages

**Obstacles et plateformes**
- Bumper
- Ventilateur
- Trampoline
- Lanceur
- Plateformes mobiles

### Éditeur VR

- Création de niveaux dans un environnement immersif
- Placement des obstacles et plateformes
- Rotation et verrouillage des objets
- Exportation des niveaux au format JSON
- Environnement partagé

### Gestion des niveaux

Le format JSON permet de transférer les données d'un niveau entre l'éditeur VR et le jeu PC.

Le fonctionnement prévu est le suivant :

1. Création du niveau dans l'éditeur VR.
2. Placement et configuration des obstacles.
3. Exportation du fichier JSON.
4. Importation du fichier dans le jeu PC.
5. Chargement et utilisation du parcours.

## Architecture du projet

Le dépôt PC est organisé selon la structure standard d'un projet Unity.

```text
Factory-Frenzy-PC/
├── Assets/             # Scènes, scripts, prefabs et ressources
├── Packages/           # Dépendances Unity
├── ProjectSettings/    # Configuration du projet
├── README.md           # Documentation du projet
└── CHANGELOG.md        # Historique des modifications
```

## Installation

### Prérequis

- Unity Hub
- Unity 2022.3.19f1
- Git
- Git LFS

### Récupération du projet

Cloner le dépôt :

```bash
git clone https://github.com/YLIssakha235/Factory-Frenzy-PC.git
```

Ouvrir ensuite Unity Hub :

1. Cliquer sur **Add project from disk**.
2. Sélectionner le dossier `Factory-Frenzy-PC`.
3. Ouvrir le projet avec Unity 2022.3.19f1.
4. Attendre l'importation des ressources.
5. Ouvrir la scène du jeu et lancer le mode **Play**.

## Organisation du développement

Le développement est réalisé en équipe de trois personnes, avec une répartition des fonctionnalités entre les parties PC et VR.

GitHub permet de centraliser le code et de suivre les modifications apportées au projet.

Notre organisation repose sur :
- Des branches de développement dédiées aux fonctionnalités
- Des commits réguliers
- L'intégration des modifications sur la branche principale
- Le suivi des tâches et de l'avancement du projet

## Projet VR

L'éditeur de niveaux VR est développé dans un dépôt GitHub séparé.

**Dépôt VR :** [Factory-Frenzy-VR](https://github.com/YLIssakha235/Factory-Frenzy-VR)

Les deux dépôts constituent les deux parties du projet Factory Frenzy.

## Équipe

Projet réalisé au **CESI Rouen - 2026**, dans le cadre du module Réalité Augmentée / Réalité Virtuelle.

| Membre | Partie | Responsabilités |
|---|---|---|
| **KAMMOUN Achraf** | PC - dev1 | Joueur et caméra, course, chronomètre, menus, import JSON et niveaux 1 à 3 |
| **YAYA LIBIS Issakha** | PC - dev2 | Bumper, ventilateur, trampoline, lanceur, plateformes mobiles et contrôleur du joueur |
| **CHAKRA Mohamad** | VR - dev3 | Éditeur de niveaux VR, placement, rotation, verrouillage, export JSON et environnement partagé |

---

**Factory Frenzy - CESI Rouen | 2026**
