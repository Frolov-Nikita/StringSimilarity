# StringSimilarity
Проект содержит метод по оценке "похожести" строк.
Нужен для определения соответсвия строк при опечатках.
Учитывает капс, лишние пробелы, в меньшей степени перестановку строк.
метод double Similar(string src, string dst) выдает оценку от 0 до 1.

## Пример: 
```
Similar("hello world!", "hello world!") = 1,0
Similar("hello world!", "   hello world!")      = 0,940741
Similar("hello world!", "HELLO WORLD!") = 0,916667
Similar("hello world!", "hllwrld")      = 0,747368
Similar("hello world!", "world hello!") = 0,654167
Similar("hello world!", "h")    = 0,192308
Similar("hello world!", "привет мир!")  = 0,126087
```