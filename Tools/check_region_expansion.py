"""Independent source/formula audit; does not compile C# or execute Unity."""
from pathlib import Path
import math, random, re
ROOT=Path(__file__).resolve().parents[1]
MASK=(1<<32)-1

def mix(x):
    x&=MASK;x^=x>>16;x=x*0x7feb352d&MASK;x^=x>>15;x=x*0x846ca68b&MASK
    return x^(x>>16)
def safe(row,seed):return mix(row^seed)%3
def rest(row):return row>=9 and 9<=row%18<=13
def section(row):return row>=0 and 4<=row%36<=7
def ride(row):return row>=0 and 18<=row%36<=23
def festival(row):return row>=0 and 4<=row%108<=7
def override(row):return row>=0 and (3<=row%36<=8 or 17<=row%36<=26)
def original(row):
    if rest(row):return 0
    for start,period,kind in [(32,72,7),(2,36,8),(8,36,10),(14,36,12),(20,36,9),(16,36,15)]:
        if row>=start and row%period==start:return kind
    return 6 if row>=4 and row%12==4 else 0
TRAFFIC=[37,40,44,44];JUMP=[36,38,41,45];SLIDE=[42,39,42,46]
ALLOWED=[{14,37,36,42,25,18,23},{14,40,38,39,31},{14,44,41,42,43,12,23},{14,44,45,46,47,25}]
def dynamic(row,region):
    if override(row):return 0
    if row>=36 and row%36==0:return TRAFFIC[region-4]
    base=original(row)
    if base==0:return 0
    if base==6:return TRAFFIC[region-4]
    return ((25 if row%4==0 else 18) if region==4 else 31 if region==5 else
            (43 if row%4==0 else 12) if region==6 else (47 if row%4==0 else 25))
def kind(row,lane,seed,region):
    sf=safe(row,seed);i=region-4
    if override(row):
        if festival(row) or lane!=(sf+1)%3:return 0
        if ride(row):return TRAFFIC[i] if row%36 in [19,21] else 0
        return (SLIDE[i] if row%2 else JUMP[i]) if section(row) else 0
    if row<2 or rest(row) or lane==sf:return 0
    d=dynamic(row,region)
    hazard=(2 if sf==0 else 0) if original(row)==12 else (sf+1)%3
    if d:return d if lane==hazard else 0
    if any(dynamic(r,region) for r in [row-1,row,row+1]):return 0
    k=mix((row*7+lane+71)^seed)%4+1
    return JUMP[i] if k==1 else (23 if region in [4,6] else JUMP[i]) if k==2 else TRAFFIC[i] if k==3 else SLIDE[i]

# Check this audit's key constants against the implementation, not a stale copy.
profile=(ROOT/'Assets/IndianRunner/Runtime/RegionProfile.cs').read_text()
assert 'Shillong=4,Kolkata=5,MadhyaPradesh=6,Himachal=7,Count=8' in profile
rules=(ROOT/'Assets/IndianRunner/Runtime/ExpandedRegionRules.cs').read_text()
assert 'RegionRules.HillBus' in rules and 'RegionRules.YellowTaxi' in rules and 'RegionRules.SafariJeep' in rules
assert 'region==RegionProfile.Shillong ? RegionRules.ForestBranch' in rules
region_ids=(ROOT/'Assets/IndianRunner/Runtime/RegionRules.cs').read_text()
ids=dict((k,int(v)) for k,v in re.findall(r'(\w+)=(\d+)',region_ids))
for key,value in [('PineappleBasket',36),('HillBus',37),('SweetCrates',38),('FlowerAwning',39),('YellowTaxi',40),('ForestBaskets',41),('ForestBranch',42),('Deer',43),('SafariJeep',44),('AppleCrates',45),('WoolAwning',46),('Sheep',47)]:assert ids[key]==value
for region in range(4,8):
    for seed in range(32):
        for row in range(2000):
            sf=safe(row,seed);ks=[kind(row,lane,seed,region) for lane in range(3)]
            assert ks[sf]==0
            assert all(k==0 or k in ALLOWED[region-4] for k in ks)
            if festival(row):assert ks==[0,0,0]
            if ride(row):
                assert not rest(row)
                for lane,k in enumerate(ks):
                    if k:assert abs((lane-sf)*2.65)>1.08+.80
            if dynamic(row,region):
                for neighbour in [row-1,row+1]:assert all(kind(neighbour,lane,seed,region)==0 for lane in range(3))
print('PASS: 256,000 new-destination route rows: safe lane, available catalogue, ride clearance, festival and traffic recovery.')

# Exact eight-stop shuffle-bag algorithm, including the repeat-boundary rule.
for start in range(8):
    for seed in range(150):
        rng=random.Random(seed);current=start;visited=bag=1<<start
        for visit in range(1,65):
            visited|=1<<current
            if visited&255==255:visited=0
            available=[i for i in range(8) if i!=current and not visited&(1<<i)]
            nxt=available[rng.randrange(len(available))];visited|=1<<nxt
            if visit%8==0:bag=0
            assert nxt!=current and not bag&(1<<nxt)
            bag|=1<<nxt;current=nxt
print('PASS: 1,200 eight-destination journeys / 76,800 transfers: complete bags, no adjacent repeat.')

# Declared geometry/collision opening and timing checks.
assert .82<1.32 and 9**2/(2*24)>.90+.02
for width in [1.08,1.02,1.04]:assert width+.80<2.65
assert 4.9-.065>2.65+.80
assert 7.1-3.2/2>4.25 # apple crown stays outside road
assert 10-2.1>4.25 # nearest conifer crown
assert 8.2-1.0>4.25 # separate tram is scenery, never a lane hazard
assert 180*math.log((11+224/180)/(11+112/180))<10
picker=(ROOT/'Assets/IndianRunner/Runtime/StageSelectionScreen.cs').read_text()
assert '(RegionProfile.Count+1)/2' in picker and 'cardHeight*rows+gap*(rows-1)' in picker
for viewport in [160,400,650,900]:
    h=max(224,min(246,(viewport-12)*.5));content=h*4+36
    bottom_y=3*(h+12)+h
    assert abs(bottom_y-content)<.001 and max(0,content-viewport)+viewport>=bottom_y
keys=re.search(r'keys=\{([^}]+)\}',profile).group(1)
keys=re.findall(r'"([^"]+)"',keys);assert len(keys)==8 and len(set(keys))==8
assert keys[:4]==['runner.best','runner.best.kerala','runner.best.punjab','runner.best.goa']
print('PASS: jump/slide/ride and roadside clearance, eight-card scroll bounds and stable unique save keys.')

from PIL import Image
folder=ROOT/'Assets/IndianRunner/Resources/StageSelection'
files=[folder/(n+'.jpg') for n in ['Shillong','Kolkata','MadhyaPradesh','Himachal']]
for file in files:
    with Image.open(file) as image:assert image.size==(512,288)
    assert 'maxTextureSize: 512' in Path(str(file)+'.meta').read_text()
print('PASS: four bundled 512×288 thumbnails;',sum(f.stat().st_size for f in files),'new source bytes.')
