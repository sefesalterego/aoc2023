# Advent of Code 2023
Hier findet Ihr Anmerkungen zu meinen Lösungen für den Advent of Code 2023. Das soll beim Verständnis des entsprechenden Codes helfen. Ich schreibe auch dazu ob und was man meiner Meinung nach aus der Aufgabe für die Praxis lernen kann.

## Tag 1

Teil 1 war ist LINQ schnell erledigt. Wer hier Vergleiche in der Form `chr >= '0' && chr <= '9'` anstellt, kennt wahrscheinlich die `Char.IsDigit`-Methode nicht.

Bei Teil 2 können mit einem `Dictionary` die Zahlworte schnell auf Ziffern umgesetzt werden. Wichtig ist allerdings, dass für das erste Zeichen von vorne und beim letzten Zeichen von hinten gesucht wird, da manchmal Zahlworte verschmolzen sein können, wie z.B. "twone". Von vorne ist das eine "two", von hinten eine "one".

**Lerneffekt:**
* Wenn es um die Verarbeitung von Listen geht, macht LINQ das Leben leichter.
* Es ist gut zu wissen, dass `Char` eine Reihe von Methoden zur Verfügung stellt, mit der die Art des Zeichens (genauer gesagt: die Unicode-Charakterklasse) ermittelt werden kann.

**Relevanz für die Praxis:** gering

## Tag 2

Hier habe ich zum Zerlegen der Eingabedaten eine Reihe von Split-Operationen eingesetzt. In der Praxs könnte man sich überliegen, ob man einen Parser schreiben würde, aber hier tut es auch die Primitivlösung. Die Prüfung von Teil 1 ist simpel. Bei Teil2 Aggregiere ich mit `Enumerable.Aggregate` alle drei Zahlen in einem Durchlauf. Das kann man auch in einer Schleife oder in drei LINQ-Durchläufen (einer für jede Farbe) mit `Enumerable.Max` machen. Mit Value Tuple geht das aber auch so (*damn you, Microsoft*).

**Lerneffekt:** Wer will, kann die Gelegenheit nutzen, sich `Enumerable.Aggregate` anzuschauen. Im praktischen Einsatz braucht man das aber nicht so oft.

**Relevanz für die Praxis:** gering

## Tag 3

Zum Auslesen der Eingabedaten habe ich hier einen Parser geschrieben (allerdings parse ich jede Zeile getrennt). Der Parser hat mehrere Outputs: ein Mehrdimensionales `Boolean`-Array, das für jede Position bestimmt, ob hier ein Teil ist, ein weiteres Array, das die Zahnräder markiert (für Teil 2) und die Teilenummern samt Position und Länge.

Für Teil 1 werden Position und Länge der Teilenummer zum Kasten erweitert, der die Nummer umgibt und in dem nach Teilen geprüft wird. Hier reicht ein Treffer und die Nummer ist gültig. Die Prüfung geht am einfachsten mit zwei geschachtelten `for`-Schleifen. Für die Summe nehme ich natürlich wieder LINQ.

Bei Teil 2 nutze ich wieder den Kasten um die Teilenummer, diesmal um alle Zahnräder um die Teilenummer herum zu ermitteln. Dann verwende ich eine LINQ-Query, die das Ergebnis in folgenden Schritten ermittelt:

* Die 1:n-Beziehung zwischen Teilenummer und Zahnrädern wird zu einer flachen 1:1-Liste umgewandelt
* Die flache Liste wird nach identischen Zahnrädern gruppiert
* Alle Gruppen, in denen nicht *genau zwei* Teilenummern enthalten, fliegen raus
* Beim Rest werden zunächst die Teilenummern multipliziert
* Daraus wird am Schluss die Summe gebildet

**Lerneffekt:**
* Es gibt auch mehrdimensionale Arrays. In der Praxis sind sie aber sehr selten.
* Es ist gut zu wissen, wie man einen Parser schreibt. In der Praxis habe ich das öfter machen müssen.
* Mit LINQ können komplexe Listenoperationen effizient erledigt werden. Allerdings können die Queries dabei schnell unübersichtlich werden.
* Ich setze hier die Query-Syntax von LINQ ein, die übersichtlicher als Method-Syntax ist. Wer `let`, mehrfache verkettete `from`-Ausdrücke, `group` und `group into` noch nicht kennt, hat die Gelegenheit, sich das mal anzuschauen.

**Relevanz für die Praxis:** insgesamt hoch. Auch wenn die Aufgabe selbst praktisch gar nicht relevant ist, so sind die einzelnen Aspekte des Lösungswegs (Parser, LINQ) wichtige Teile des Werkzeugkastens eines C#-Programmierers.

## Tag 4

Die Aufgabe in Tag 4 ist die perfekte Gelegenheit, sich mit `HashSet`s vertraut zu machen. In Teil 1 geht es darum, die Schnittmenge zwischen den vorhandenen und den Gewinnerkarten zu ermitteln. Solche Mengenoperationen sind die Kernkompetenz von Hashsets. Klar kann man das auch mit LINQ machen, aber LINQ ist hier nur die zweitbeste Lösung.

Beim zweiten Teil werden in einer `for`-Schleife die Zahl der Karten ermitttelt. Hier ist es nützlich, dass die Werte einer Zeile nur eine Auswirkung für die Folgezeilen haben. Es muss also nicht nach hinten geschaut werden.

**Lerneffekt:** Hashsets. Hashsets. Hashsets.

**Relevanz für die Praxis:** mittel. Hashsets kommen in der Praxis durchaus vor. Man sollte sie einzusetzen wissen.

## Tag 5

In Tag 5 geht es in erster Linie um den Vergleich von Werten gegen Wertebereiche. Im ersten Teil kann das straightforward durchgeführt werden. Man nimmt den Wert und wendet die Bereiche nacheinander an. Die Bereiche habe ich hier in die `Map`- und `MapSet`-Strukturen organisiert, in denen die Prüffunktionalität gekapselt ist.

Teil 2 war um einiges aufwendiger. Dass man die Werte der einzelnen Bereiche nicht einzeln prüfen kann, war von Anfang an klar. Daher bin ich dazu übergegangen, die Bereiche gegeneinander zu vergleichen. Da sich die Bereiche überlappen, müssen sie bei dem Vorgang auch zerlegt werden. Das habe ich auch in meiner ersten Lösung auch gemacht. Bei der Menge der Bereiche führt das Zerlegen hzu einer Inflation von Prüfungen, weshalb die Berechnung einige Sekunden gedauert hat. Egal, hauptsache es gibt ein Ergebnis. Das war aber leider falsch.

Für den erfolgreichen Lösungsansatz habe ich zuerst geprüft, ob die Bereiche des letzten `MapSet`s (*humidity to location*) lückenlos sind (das dafür geschriebene Tool ist nicht Teil der Lösung, die ich eingecheckt habe). Bingo, die Bereiche grenzen direkt aneinander an. Das macht es einfach, die letzte Map mit der vorherigen zu verschmelzen. Auch wenn der vorherige Bereich nicht lückenlos ist, kann damit recht einfach umgegangen werden. So können dann alle Maps von unten nach oben verschmolzen werden. Auch dabei werden Bereiche zerlegt, aber die Menge steigt nicht zu stark an.

Wenn die Bereiche erstmal zusammengefasst sind, geht die Ermittlung der Lösung sehr schnell. Damit ist auch die ursprünglich schlechte Performance gelöst.

**Lerneffekt:**
* Hier kann man das Vergleichen mit Bereichen und das Abgleichen von Bereichen zueinander üben. Das zu beherrschen ist wichtig, da dies oft in der Praxis vorkommt (z.B. bei Datumsangaben).
* Die direkte Lösung ist nicht immer die beste. Manchmal muss man sich etwas besseres ausdenken.

**Relevanz für die Praxis:** hoch (wegen der Prüfung gegen Bereiche)

## Tag 6

Wer bei Tag 6 sofort anfängt, alle möglichen Rennen durchzuspielen, findet nicht die beste Lösung. Die Aufgabe ist eigentlich ein mathematisches Problem und lässt sich entsprechend lösen. Für die Laufzeit gilt folgende Formel:

$(t - x) \cdot x > r$

Dabei ist $t$ die zur Verfügung stehende Gesamtzeit, $r$ der bestehende Rekord und $x$ die Zeit, die der Knopf gedrückt wird (das gesuchte Ergebnis).

Das lässt sich zu dieser quadratischen Gleichung umformen:

$x^2 - tx + r < 0$

Grafisch lässt sich diese Formel als Parabel darstellen, die die x-Achse an zwei Stellen schneidet. Für das Bespiel aus der Aufgabenstellung mit $t = 7$ und $r = 9$ sieht das so aus:

![image](https://github.com/sefesalterego/aoc2023/assets/34250217/4bf91cd3-bb21-4460-a092-da6d6aaa8d6e)

Die Werte zwischen den beiden Schnittpunkten mit der x-Achse (2, 3, 4 und 5) sind das gesuchte Ergebnis, da hier das entsprechende y kleiner als 0 ist (siehe die quadratische Gleichung oben, da steht $< 0$ auf der rechten Seite).

Die beiden x lassen sich mit der Mitternachtsformel schnell ermitteln (da passt es, dass ich die Aufgabe um Mitternacht herum gelöst und eingecheckt habe):

$x_{1,2} = \frac{t \pm \sqrt{t^2 - 4r}}{2}$

Für jedes Rennen ist die Lösung damit eine O(1)-Operation.

**Lerneffekt:** Hier zeigt sich, dass es wichtig ist, sich Zeit zu nehmen, das Problem genau zu analysieren, bevor man mit dem Programmieren beginnt.

**Relevanz für die Praxis:** gering

## Tag 7

Bei Tag 7 geht es in erster Linie darum, die Eingangsdaten zu gewichten und zu sortieren. Beim Thema sortieren in .NET ist in der Rgele das `IComparable<T>`-Interface relevant. Alle Implementierungen werden dann als sortierbar erkannt, z.B. von `List<T>.Sort`. Also frisch ans Werk und `IComparable<T>` implementiert. Damit das funktioniert, passieren beim Erstellen der sortierbaren Instanzen zwei Dinge: Der Wert des Bildes wird ermittelt und die Symbole für die einzlen Karten (A, K, Q, J, T, 9 usw.) werden auf andere Zeichen gemapt, die der regulären Sortierreihenfolge entsprechen. Dann in eine Liste, diese sortieren und die Wetten mit der Position in der Liste multiplizieren - fertig.

Der zweite Teil ist im Prinzip identisch, nur dass hier die Joker berücksichtigt werden.

**Lerneffekt:**
* Man kommt dazu, mal das `IComparable<T>`-Interface zu implementieren.
* Man kann eine `enum` für den Kartenwert erstellen und auch für die Sortierung verwenden.

**Relevanz für die Praxis:** mittel. `IComparable<T>` (und wenn wir schon dabei sind auch `IEquatable<T>`) sollte man kennen und implementieren können.

## Tag 8

Ach ja, Tag 8. Der erste Teil kann direkt implementiert werden. Nur die einzelnen Knoten lesen und dann nach der LR-Reihenfolge durchgehen, fertig. Alles also ganz einfach.

Ganz anders der zweite Teil. Das Vorgehen des ersten Teils funktioniert hier nicht, dafür sind es viel zu viele Durchläufe. Was habe ich alles versucht, den Vorgang hier zu optimieren: Gruppieren, Caching usw., nichts hat geholfen. Allerdings sind mir bei meinen Debugsessions zwei Dinge zu den Ausgangsdaten zwei Dinge aufgefallen:

1. Jeder Ausgangspunkt \*\*A führt genau zu einem Endpunkt \*\*Z. Geht man dann weiter, kommt man immer wieder zum selben Endpunkt, und zwar in immer der gleichen Zahl von Schritten. Die Zahl der Schritte ist allerdings für jede Kombination aus Anfangs- und Endpunkt unterschiedlich.
2. Wenn man am Endpunkt ankommt ist man immer am Ende der vorgegebenen LR-Reihenfolge. Der nächste Durchlauf fängt immer wieder am Anfang an. Damit ist sichergestellt, dass alle Durchläufe mit derselben LR-Folge durchgeführt werden können.

Diese Besonderheit bei den Ausgangsdaten ermöglicht einen einfachen Lösungsweg: Jede Kombination von Ausgangs- und Endpunkt kann getrenn durchlaufen und die Zahl der Durchläufe registriert werden. Das kleinste gemeinsame Vielfache (kgV) dieser Durchlaufzahlen ist die kleinste Zahl von Schritten und damit das gesuchte Ergebnis. Das kgV kann man unter Zuhilfenahme des größten gemeinsamen Teilers (ggT) ermitteln. Für den ggT gibt es den [euklidischen Algorithmus](https://de.wikipedia.org/wiki/Euklidischer_Algorithmus). Am Ende dauert die gesamte Ermittlung des Ergebnisses nur einen Sekundenbruchteil.

**Lerneffekt:**
* Der einfachste Lösungsweg funktioniert nicht immer bei größeren Datenmengen. In solchen Fällen muss eine optimalere Lösung her.
* Es lohnt sich, auf die Besonderheiten des zu lösenden Problems zu schauen. Die oben genannen Punkte sind hier die entscheidende Vereinfachung, die eine effiziente Lösung ermöglicht.

**Relevanz für die Praxis:** gering

## Tag 9

Dieses Promlem kann ohne weitere Umschweife nach dem in der Beschreibung dargestellten Algorithmus gelöst werden. Der zweite Teil bietet hier nichts neues, man macht das gleiche auf der anderen Seite (ich habe mich zunächst gefragt, ob ich was übersehe). Na ja. Wenigstens sind das zwei leicht verdiente Sterne.

**Lerneffekt:** keiner

**Relevanz für die Praxis:** gering