filename = "surnames.txt"
names = []

with open(filename, "r") as fh:
    names = fh.readlines()

names = list(set(names))

with open(f"unduped_{filename}", "w+") as fh:
    fh.writelines(names)
