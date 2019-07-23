# E-Fame-Watch

This is a cool little application, that can take some info from a URL and an XPath, and display it with some statistical data.
Features a normal Column Chart and a Box Chart. 

With the program is an example of some of my workshop items from Steam.

![Error Loading Image](https://yxwzcq.db.files.1drv.com/y4mqGsqjehHGMVyAcnnPluqjUlMN9adgqWV0Fc9VJLDOebZncZBVIJZvcnVoSJpAJmI9skfOS9SywEaJRZjPLJ_XAA2bm0ko3994WYmDOtIkhbHptUSYe4NXsZfLjgrf2MJe0u7WF12AMvOHxTv8VP0fJP05eYTJsFV5YX1yZY0wtpsnTGTlLUqAThuAgQLm65ZZCeA_lRy6xgkWy79EAMQFA?width=520&height=892&cropmode=none)
![Error Loading Image](https://k5pkkw.db.files.1drv.com/y4mDeKg3uLdI_kKT2SC6Slg_J3s2h4J-sAuE91oOVUezKRXCSa53luM4AdtXgck5k_U1KquZyLEb0mBDYMgtb12rPWziStf8i2ezrX2YYP1hdEz9CaqZ1FhJmNVx4LuukZXN-GwQ96-mJX6OMb6_5_3msKKwYzIvx-uCcLOEf0uxIyQJCiSTN4gC43zkeAbvDkxNq36AfJB0rjWxlx0b5RdRw?width=260&height=532&cropmode=none)

This program uses an external package, called HTMLAgilityPack, that can be found here https://html-agility-pack.net/

## How to find an XPath?

In Google Chrome you first find the web page you want to take a value from, in this example i will use one of my own steam workshop items:

![Error Loading Image](https://xnxkiq.db.files.1drv.com/y4mae1Qcd0z7A7BRhW7oeAiSwl9-mHI0nxDvwqCbso9FEZHeng7imyrdiMkiFEfb5zLHbhvmaAVgp2p0ffk56Rs2b0kelh_nONor0lo9lp6jjZTcTYKLRYTOTwmPPqPASkWXnGVCMPRkVwh-3QebilTsEXX9-iOUtAP7cYoiNcWNB62uFk45EWLuQxPmy5ROC6RCzYZAqlZ04SvsvQXGxftGQ?width=1447&height=823&cropmode=none)

After that press F12 to bring up the web dev tools:

![Error Loading Image](https://wwejfg.db.files.1drv.com/y4m5ii-DP4MFuRXNzDYeW1Vh7y_YZJ3C7UIh730TlHhvpYKzAk-qtD0eUwAScRP4npQTk8HTwUPKpxcr3agnlhzNg8HEwSdW2SOe_SWwha5zV0JTWWXZHnbzSdoCELgYV4JIP_y60mys_Bz2PTMx0og1o5yMLBrCpjIGGYEgXeFkc3LNFGy7voP9gQQGbOhYOgCeMxDd54GAWhQnpqA2DE2dw?width=1447&height=822&cropmode=none)

Then click on the inspect button:

![Error Loading Image](https://kjp72w.db.files.1drv.com/y4mhiYkwRESAdMe-_rY25LR5BV4J0uRHnLCWX412DfW8-2UAsqwba_lPqCCgXrh6tq-ZsYCIr7zfINfkTb7YNS3FNDN5qPPzsO9WENAlFx2Qc6InWBrWv7EbNCfHTakg5lMQlnJ_umfrIo1CpECIRKnJSo9oIQ4D4qyDw7YY9_7avvHvgvy5git2ZSLweSMI08p5cbo-Eyl4OA8iBYZTcFFEw?width=1448&height=822&cropmode=none)

Then click on the element in the web page you want:

![Error Loading Image](https://jr5uoq.db.files.1drv.com/y4m8w1v35MPN6UH6jHLboxPFJ3UNmmuyQj-aSVR9l1njz5SY19e5z1BzopZcLtX_9R8VvH8G3pWSk5ATwaBmMGRltgqoNv-Vbm2bsIPefFRZGJw9jDWu_IQRZG1UE7K4dJxl5yfO23W_n25ilirjEWbTANQzZ3Id6y02YGKGcECRczJB87HY8Pn-3_p5fBIqFfH2pr_Mi6g7MeBwAL2y7eCBw?width=1448&height=822&cropmode=none)

Then over in the HTML window right click the element, and under copy, select copy xpath:

![Error Loading Image](https://8qmslg.db.files.1drv.com/y4mBmPGatWQzCsksjUzDAzpHBtJPQmaoi41pnyciVNjnQ_yD54N_zJo2cRzKrepQRtRa7MACLfzIN8eXjibGowGh-aELbUBa1nULAmwCELIcD3DrdksWGll5UsPAX8ZrN9vRw6P269pU4KNrG1ZcyXzEkpTPpoYWtGwiQXiiCWRYvxLXH4qyiV_-gj4rWJy4OFVa1sJHq4uKWNAWJo7hejC4g?width=1448&height=822&cropmode=none)

After this you can just simply paste the xpath to the E-Fame watch program.
