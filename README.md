# 🍬 Bucuria: Sweet Delivery

## 📌 Descriere

**Bucuria: Sweet Delivery** este un joc de tip **endless runner procedural** dezvoltat în Unity, în care lumea se generează dinamic în timp real, combinând biome-uri diferite, obstacole variate și un sistem de progresie bazat pe deblocări.

Jucătorul controlează un personaj care aleargă prin medii variate (biome), unde trebuie să evite obstacole, să colecteze obiecte și să supraviețuiască cât mai mult. Lumea este generată pe bază de **grid modular**, cu tranziții fluide între biome-uri și distribuție controlată a obiectelor prin sistem de **weight-based spawning**.

---

## 🎮 Gameplay

* 🏃 Endless runner complet procedural
* 🌍 Generare dinamică de biome-uri (Forest, City etc.)
* 🧱 Sistem modular pe grid (segmente uniforme)
* ⚖️ Spawn system bazat pe **weight (densitate reală a obiectelor)**
* 🚧 Obstacole mixate între biome-uri în funcție de progres și deblocări
* 🔓 Sistem de deblocare a conținutului prin Shop / progres
* 🎧 Audio adaptiv în funcție de biome
* 🔄 Tranziții între biome-uri fără întreruperi vizuale
* 🧠 Generare deterministă (seed-based) pentru consistență

---

## 🌍 Sistem de lume (World Generation)

Proiectul folosește un sistem avansat de generare procedurală:

### 🔹 Biome System

* Fiecare biome are:

  * lungime variabilă (min / max segments)
  * set propriu de obiecte
  * prefab de tranziție
* Biome-urile sunt selectate dinamic în funcție de progres și unlock-uri

### 🔹 Grid-based spawning

* Lumea este construită pe `segmentLength`
* Toate pozițiile sunt aliniate pe un grid fix
* Elimină:

  * gaps între biome-uri
  * suprapuneri
  * jitter vizual

---

## ⚖️ Sistem de Spawn (Important)

* `weight` controlează **densitatea și frecvența reală** a obiectelor
* obiectele din biome-uri diferite sunt **amestecate natural**
* obiectele globale (ex: BASE obstacles) apar în toate biome-urile
* biome-specific objects apar doar în biome-ul lor

👉 Rezultat: lumea pare organică, dar rămâne controlabilă

---

## 🧠 Arhitectură

* Separare între:

  * World Generator (biome + grid)
  * Runner Generator (obstacles + gameplay)
  * Environment System (audio + biome tracking)
* Sistem fără dublări inutile de logică
* Generare deterministă pe seed
* Cleanup automat pentru optimizare

---

## 🔊 Audio System

* Muzica și ambientul se schimbă în funcție de biome
* Detectarea biome-ului este bazată pe poziția reală a playerului
* Tranziții audio fluide (fără spam / fără restart inutil)

---

## 🧱 Tehnologii utilizate

* Unity Engine
* C#
* Procedural Generation Systems
* Custom Game Architecture
* Physics-based detection (raycast placement)

---

## 🚀 Obiectivele proiectului

* Crearea unui endless runner modern, extensibil și modular
* Construirea unui sistem de lume procedurală realist și controlabil
* Integrarea gameplay-ului cu progresie și unlock-uri
* Optimizare pentru rulare continuă (infinite world streaming)

---

## 📷 Capturi de ecran

### Meniu principal
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/24d702ee-43b7-4dce-a8e2-8f4b1201234c" />

### Magazin
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/21fc040e-76d0-4784-a4e9-0985926ce576" />

### Inventar
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/3015a38e-ee48-46a3-88f1-e003c7fd5e66" />

### Statistici
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/3d0bf21d-3a74-4b5d-b0d5-6f1ce28e647e" />

### Setări
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/a94ad595-b63f-4f41-bdf6-28b13322c26d" />

### Joc
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/8e50bc57-e068-49ef-a185-cfdc46ade042" />

### Biom: Oraș
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/736452e0-8823-44e7-9b4b-1a214b026d14" />

### Biom: Pădure
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/b712da32-2f66-42ef-9a03-8ca462091535" />

### Panou Game Over
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/159daac7-8a53-41f9-a69e-ca87b588ba82" />

### Pauză
<img width="1750" height="980" alt="image" src="https://github.com/user-attachments/assets/80585fcf-c79b-4eca-a759-cf96d39fb90c" />

---

## 📦 Build

[⬇ Download latest release](https://github.com/USERNAME/REPO/releases/latest)

---

## 📄 Raport

📘 Raport complet al proiectului:

[⬇ Descarcă raportul PDF](docs/Raport_Practica_Bucuria_Sweet_Delivery.pdf)

[⬇ Descarcă raportul DOCX](docs/Raport_Practica_Bucuria_Sweet_Delivery.docx)

---

## 👤 Autor

Victor Bejuc

---

## 📄 Licență

Acest proiect este creat în scopuri educaționale și de portofoliu.
