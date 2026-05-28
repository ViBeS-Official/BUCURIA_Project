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

*(Adaugă screenshots aici – biome transitions, gameplay, shop etc.)*

---

## 📦 Build

[⬇ Download latest release](https://github.com/USERNAME/REPO/releases/latest)

---

## 👤 Autor

Victor Bejuc

---

## 📄 Licență

Acest proiect este creat în scopuri educaționale și de portofoliu.
