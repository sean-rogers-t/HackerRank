# Re-implementing the Fenwick Tree class in Python for demonstration purposes
class FenwickTree:
    def __init__(self, size):
        self.tree = [0] * (size + 1)

    def update(self, i, val):
        while i < len(self.tree):
            self.tree[i] += val
            i += i & -i

    def query(self, i):
        sum = 0
        while i > 0:
            sum += self.tree[i]
            i -= i & -i
        return sum

# Initialize and populate the Fenwick Tree for the new example tree
fenwick_tree_example = FenwickTree(5)  # Size 5 to include the 0 index
fenwick_tree_values_example = [0, 0, 2, 4, 4]   #Fenwick tree based on the corrected logic

for i, val in enumerate(fenwick_tree_values_example):
    if i>0:
        fenwick_tree_example.update(i, val)

# DFS to Node Mapping for the new example tree
dfs_to_node_mapping_example = {1: 2, 2: 1, 3: 3, 4: 4}

# Dummy function for FindLCA for the new example tree
def FindLCA_example(A, B):
    return 1  # LCA of 3 and 4 in this example is 1

# Query function for the new example tree
def Query_example(A, B):
    # Step 1: Find LCA
    lca = FindLCA_example(A, B)
    
    # Step 2: Query path from A to LCA
    dfs_number_of_A = dfs_to_node_mapping_example[A]
    dfs_number_of_LCA = dfs_to_node_mapping_example[lca]
    sum_A_to_LCA = fenwick_tree_example.query(dfs_number_of_A) - fenwick_tree_example.query(dfs_number_of_LCA - 1)
    
    # Step 3: Query path from B to LCA
    dfs_number_of_B = dfs_to_node_mapping_example[B]
    sum_B_to_LCA = fenwick_tree_example.query(dfs_number_of_B) - fenwick_tree_example.query(dfs_number_of_LCA - 1)
    
    # Step 4: Combine sums, avoid double-counting LCA
    lca_value = fenwick_tree_example.query(dfs_number_of_LCA) - fenwick_tree_example.query(dfs_number_of_LCA - 1)
    total_sum = sum_A_to_LCA + sum_B_to_LCA - lca_value
    
    return total_sum

# Run Query(3, 4) for the new example tree
print(Query_example(3, 4))
