#define _CRT_SECURE_NO_WARNINGS
#include <stdio.h>
#include <stdlib.h>

double** getData(int n) 
{
    double** k;
    k = new double* [2];
    k[0] = new double[n];
    k[1] = new double[n];
    for (int i = 0; i < n; i++) 
    {
        k[0][i] = (double)i;
        k[1][i] = 8 * (double)i - 3;
        k[1][i] = 8 * (double)i - 3 + ((rand() % 100) - 50) * 0.05;
    }
    return k;
}

void getApprox(double** x, double* a, double* b, int n)
{
    double sumx = 0;
    double sumy = 0;
    double sumx2 = 0;
    double sumxy = 0;
    for (int i = 0; i < n; i++) {
        sumx += x[0][i];
        sumy += x[1][i];
        sumx2 += x[0][i] * x[0][i];
        sumxy += x[0][i] * x[1][i];
    }
    *a = (n * sumxy - (sumx * sumy)) / (n * sumx2 - sumx * sumx);
    *b = (sumy - *a * sumx) / n;
    return;
}
int main() 
{
    double** x, a, b;
    int n;
    system("chcp 1251");
    system("cls");
    printf("Введите количество точек: ");
    scanf("%d", &n);
    x = getData(n);
    for (int i = 0; i < n; i++)
        printf("%5.1lf - %7.3lf\n", x[0][i], x[1][i]);
    getApprox(x, &a, &b, n);
    printf("a = %lf\nb = %lf", a, b);
    getchar();
    return 0;
}
