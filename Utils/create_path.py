#!interpreter
#!./venv/bin/python3.10

import numpy as np
import matplotlib.pyplot as plt
from matplotlib.patches import Ellipse

import os.path
import sys

if len(sys.argv) >= 5:
    print("Using CLI parameters")
    dir = sys.argv[1]
    airstrip_len = float(sys.argv[2])
    airport_zone = float(sys.argv[3])
    enter_routes = int(sys.argv[4])
else:
    print("Using default parameters")
    dir = "/Users/iliat/RiderProjects/AirportSim/Logs"
    airstrip_len = input("Enter airstrip length     (m):")
    airport_zone = input("Enter airport zone radius (m):")
    enter_routes = input("Enter entering routes count  :")

    airstrip_len = float(airstrip_len) if airstrip_len != '' else 300
    airport_zone = float(airport_zone) if airport_zone != '' else 1000
    enter_routes = int(enter_routes) if enter_routes != '' else 3


f, axes = plt.subplots(1, 1, figsize=(8, 8))
i = 0
colors = plt.cm.Set1(np.linspace(0, 1, 10))

for file in os.listdir(dir):
    if not file.endswith(".csv"): continue

    i += 1
    plane_name = file.split("/")[-1].replace(".csv", "")
    t = np.loadtxt(f"{dir}/{file}", delimiter=";", usecols=(0,), dtype=str)
    data = np.loadtxt(f"{dir}/{file}", delimiter=";", usecols=(1, 2, 3, 4, 5))

    status_change = np.where(data[:-1, -1] != data[1:, -1])[0]
    angle_change = np.where(data[:-1, 3] != data[1:, 3])[0]

    axes.scatter(data[status_change, 0], data[status_change, 1], color=colors[i % 10])
    #axes.scatter(data[angle_change, 0], data[angle_change, 1], color=colors[i % 10], s=5)

    axes.plot(data[:, 0], data[:, 1], label=plane_name, color=colors[i % 10], marker='o', ms=2)
    axes.text(data[0, 0], data[0, 1] + 50, plane_name, color=colors[i % 10])

    for j in status_change:
        axes.text(data[j, 0] + 80, data[j, 1] - 80, t[j], alpha=0.9, fontsize='x-small', color=colors[i])

axes.add_patch(Ellipse(xy=(0, 0), width=airport_zone*2, height=airport_zone*2,
                       edgecolor='gray', fc='gray', alpha=0.1, lw=2, label="Airport zone"))
axes.plot((-airstrip_len/2, airstrip_len/2), (0, 0),  marker="s", lw=8, color="gray")

for route in range(enter_routes):
    angle = 360.0 / enter_routes * route
    xy = airport_zone * np.cos(angle), airport_zone * np.sin(angle)
    lz = airstrip_len/2 if xy[0] > 0 else -airstrip_len/2, 0
    axes.plot([xy[0], lz[0]], [xy[1], lz[1]], "--", color="black", lw=1)
    axes.scatter(xy[0], xy[1], marker="o", s=20, color="black")


axes.grid()
axes.set_xlabel("$x$, meters")
axes.set_ylabel("$y$, meters")
axes.legend()
axes.set_aspect('equal', adjustable='datalim')
plt.tight_layout()

plt.savefig("report.png")
print(f"Report saved at {os.path.dirname(os.path.realpath(__file__))}/report.png")
plt.show()





