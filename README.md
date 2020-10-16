# prng-utilities

A utility tool for working with psudo random generated numbers.

# Setup
1. Download the files from the Releases tab
2. Grab a dictionary file, I used [this one](https://github.com/dwyl/english-words) and drop it in the folder with the .exe
3. Profit

# Examples
``` prng_utilities.exe -o -k 123 -n 512 ```
Output a random number 512 characters long generated with the key 123.

``` prng_utilities.exe -e -k 123 -s "Hello World" ```
This will encrypt the string "Hello World" with the key 123

``` prng_utilities.exe -d -k 123 -s "SOME STRING TO DECRYPT" ```
This will decrypt the string "SOME STRING TO DECRYPT" using the key 123

``` prng_utilities.exe -b -s "SOME STRING TO DECRYPT" ```
Bruteforce the key using a dictionary attack that checks every word in the string.
